using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel; // 請確保已透過 NuGet 安裝 ClosedXML

namespace ETF_Compare_2._0
{
    public partial class Form1 : Form
    {
        // 定義資料結構
        public class StockItem
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public decimal Shares { get; set; }
            public string Weight { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }
        // --- 4. 讀取 Excel 方法 (封裝好的強大邏輯) ---
        private List<StockItem> ReadXlsx(string filePath)
        {
            var list = new List<StockItem>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet(1);
                var rows = sheet.RangeUsed().RowsUsed();
                bool isHeaderFound = false;

                foreach (var row in rows)
                {
                    string cell1 = row.Cell(1).GetValue<string>().Trim();
                    if (!isHeaderFound)
                    {
                        if (cell1.Contains("股票代號")) isHeaderFound = true;
                        continue;
                    }

                    // 清洗代號 (處理 "15 2330" -> "2330")
                    string symbol = cell1.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    if (string.IsNullOrEmpty(symbol) || !Regex.IsMatch(symbol, @"^\d+$")) continue;

                    // 解析股數
                    string rawShares = row.Cell(3).GetValue<string>();
                    decimal.TryParse(Regex.Replace(rawShares, @"[^\d.]", ""), out decimal shares);

                    list.Add(new StockItem
                    {
                        Symbol = symbol,
                        Name = row.Cell(2).GetValue<string>().Trim(),
                        Shares = shares,
                        Weight = row.Cell(4).GetValue<string>().Trim()
                    });
                }
            }
            return list;
        }

        // --- 1. 瀏覽按鈕：選擇昨日檔案 ---
        private void btnBrowseYesterday_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xlsx;*.xls|CSV Files|*.csv|All Files|*.*";
                ofd.Title = "請選擇昨日的 ETF 檔案";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtYesterday.Text = ofd.FileName; // 將路徑填入對應的 TextBox
                }
            }
        }

        // --- 2. 瀏覽按鈕：選擇今日檔案 ---
        private void btnBrowseToday_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xlsx;*.xls|CSV Files|*.csv|All Files|*.*";
                ofd.Title = "請選擇今日的 ETF 檔案";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtToday.Text = ofd.FileName; // 將路徑填入對應的 TextBox
                }
            }
        }

        // --- 3. 比對按鈕：核心運算邏輯 ---
        private async void btnCompare_Click_1(object sender, EventArgs e)
        {
            string pathYesterday = txtYesterday.Text.Trim();
            string pathToday = txtToday.Text.Trim();

            // 檢查路徑
            if (string.IsNullOrEmpty(pathYesterday) || !File.Exists(pathYesterday))
            {
                MessageBox.Show("請選擇有效的昨日檔案路徑！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(pathToday) || !File.Exists(pathToday))
            {
                MessageBox.Show("請選擇有效的今日檔案路徑！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnCompare.Enabled = false; // 運算時禁用按鈕
                btnCompare.Text = "分析中...";

                // 使用 Task.Run 讓 Excel 讀取在背景執行，視窗才不會卡死
                var yesterdayData = await Task.Run(() => ReadXlsx(pathYesterday));
                var todayData = await Task.Run(() => ReadXlsx(pathToday));

                if (yesterdayData.Count == 0 || todayData.Count == 0)
                {
                    MessageBox.Show("讀取失敗，請確認 Excel 檔案內容是否包含「股票代號」！");
                    return;
                }

                // $O(1) 比對：建立昨日字典
                var yesterdayDict = yesterdayData.ToDictionary(s => s.Symbol);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"ETF 持股變動報告 - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"昨日檔案：{Path.GetFileName(pathYesterday)}");
                sb.AppendLine($"今日檔案：{Path.GetFileName(pathToday)}");
                sb.AppendLine(new string('-', 60));
                sb.AppendLine($"{"狀態",-6} {"代號",-10} {"名稱",-20} {"今日股數",-12}");
                

                int changeCount = 0;
                foreach (var current in todayData)
                {
                    if (yesterdayDict.TryGetValue(current.Symbol, out var prev))
                    {
                        if (current.Shares > prev.Shares)
                        {
                            sb.AppendLine($"增加   {current.Symbol,-10} {current.Name,-20} {current.Shares,12:N0}");
                            changeCount++;
                        }
                    }
                    else
                    {
                        sb.AppendLine($"新進   {current.Symbol,-10} {current.Name,-20} {current.Shares,12:N0}");
                        changeCount++;
                    }
                }

                // 存檔到桌面
                // 修正後的路徑設定
                string outputDir = Path.Combine(Application.StartupPath, "ETF_Output");

                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
                // 設定完整的檔案路徑
                string outputPath = Path.Combine(outputDir, $"Analysis_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                // 4. 補充文字檔結尾訊息
                sb.AppendLine(new string('-', 60));
                sb.AppendLine($"比對完成！發現 {changeCount} 筆變動。");
                sb.AppendLine($"報告存於：{outputPath}");
                // 5. 寫入檔案
                File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);

                // 6. 準備訊息內容 (合併 成功通知 + 詢問)
                string message = $"比對完成！\n" +
                                 $"發現 {changeCount} 筆變動。\n\n" +
                                 $"報告已儲存於：\n{outputPath}\n\n" +
                                 $"請問是否要立即開啟輸出資料夾？";

                // 7. 彈出 Yes/No 選擇視窗
                // MessageBoxButtons.YesNo 會顯示「是」與「否」按鈕
                // MessageBoxIcon.Question 會顯示問號圖示
                DialogResult result = MessageBox.Show(message, "比對完成", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // 8. 判斷使用者的選擇
                if (result == DialogResult.Yes)
                {
                    // 只有當使用者選「是」的時候，才開啟資料夾
                    System.Diagnostics.Process.Start("explorer.exe", outputDir);
                }

                // 如果選「否」，程式就會直接往下執行 (也就是不做任何事)，不會打開資料夾
            }
            catch (Exception ex)
            {
                MessageBox.Show($"執行發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCompare.Enabled = true;
                btnCompare.Text = "比對";
            }
            // 1. 清除昨日路徑文字框
            txtYesterday.Text = "";

            // 2. 清除今日路徑文字框
            txtToday.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtYesterday.Text = ""; // 清空文字
                                    // txtYesterday.Clear(); // 用這行也可以，效果一樣
            txtYesterday.Focus();  // 讓游標跳回格子裡閃爍
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtToday.Text = ""; // 清空文字
            txtToday.Focus();  // 讓游標跳回格子裡閃爍
        }
    }
}