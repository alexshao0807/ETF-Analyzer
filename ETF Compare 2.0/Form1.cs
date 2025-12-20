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

            // --- Log: 紀錄程式啟動 ---
            WriteLog("程式啟動 - ETF Analyzer v1.0 Ready.");
            // --- 新增這段：啟用拖放功能 ---

            // 1. 開啟文字框的拖放屬性
            txtYesterday.AllowDrop = true;
            txtToday.AllowDrop = true;

            // 2. 綁定事件 (兩個框框共用同一個處理邏輯)
            // 當檔案拖進來時
            txtYesterday.DragEnter += Txt_DragEnter;
            txtToday.DragEnter += Txt_DragEnter;

            // 當檔案放開時
            txtYesterday.DragDrop += Txt_DragDrop;
            txtToday.DragDrop += Txt_DragDrop;
        }

        // --- ★ 新增功能: 寫入 Log 檔案的方法 ---
        private void WriteLog(string message)
        {
            try
            {
                // 1. 設定 Logs 資料夾路徑 (在 exe 旁邊)
                string logDir = Path.Combine(Application.StartupPath, "Logs");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                // 2. 設定檔名 (每天一個檔案，例如 Log_20231025.txt)
                string logPath = Path.Combine(logDir, $"Log_{DateTime.Now:yyyyMMdd}.txt");

                // 3. 組合 Log 內容: [時間] 訊息
                string logContent = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";

                // 4. 附加寫入檔案 (Append)
                File.AppendAllText(logPath, logContent, Encoding.UTF8);
            }
            catch
            {
                // 如果連 Log 都寫不進去 (例如權限不足)，就默默略過，不要讓程式崩潰
            }
        }
        // --- 4. 讀取 Excel 方法 ---
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
                WriteLog("警告: 昨日路徑無效或空白"); // Log
                return;
            }
            if (string.IsNullOrEmpty(pathToday) || !File.Exists(pathToday))
            {
                MessageBox.Show("請選擇有效的今日檔案路徑！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                WriteLog("警告: 今日路徑無效或空白"); // Log
                return;
            }

            try
            {
                WriteLog("開始執行比對分析..."); // Log                
                btnCompare.Enabled = false; // 運算時禁用按鈕
                btnCompare.Text = "分析中...";

                // 使用 Task.Run 讓 Excel 讀取在背景執行，視窗才不會卡死
                var yesterdayData = await Task.Run(() => ReadXlsx(pathYesterday));
                var todayData = await Task.Run(() => ReadXlsx(pathToday));

                WriteLog($"檔案讀取完成。昨日筆數: {yesterdayData.Count}, 今日筆數: {todayData.Count}"); // Log

                if (yesterdayData.Count == 0 || todayData.Count == 0)
                {
                    string errMsg = "讀取失敗，資料筆數為 0，請確認 Excel 內容。";
                    MessageBox.Show("讀取失敗，請確認 Excel 檔案內容是否包含「股票代號」！");
                    WriteLog($"錯誤: {errMsg}"); // Log
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


                WriteLog($"比對邏輯完成，發現 {changeCount} 筆變動。"); // Log
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
                WriteLog($"報告已存檔: {outputPath}"); // Log

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
                    WriteLog("使用者選擇開啟資料夾。"); // Log
                }

                // 如果選「否」，程式就會直接往下執行 (也就是不做任何事)，不會打開資料夾
            }
            catch (Exception ex)
            {
                WriteLog($"[嚴重錯誤 Exception] {ex.Message}\n堆疊: {ex.StackTrace}");
                MessageBox.Show($"執行發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCompare.Enabled = true;
                btnCompare.Text = "比對";
                txtYesterday.Text = "";
                txtToday.Text = "";
                WriteLog("作業結束，重置介面。"); // Log
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtYesterday.Text = ""; // 清空文字
                                    // txtYesterday.Clear(); // 用這行也可以，效果一樣
            txtYesterday.Focus();  // 讓游標跳回格子裡閃爍
            WriteLog("使用者手動清除昨日路徑"); // Log
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtToday.Text = ""; // 清空文字
            txtToday.Focus();  // 讓游標跳回格子裡閃爍
            WriteLog("使用者手動清除今日路徑"); // Log
        }
        // --- 拖放功能區域 Start ---

// 當滑鼠拖著東西「進入」文字框範圍時觸發
private void Txt_DragEnter(object sender, DragEventArgs e)
{
    // 檢查拖進來的是不是「檔案」(忽略網頁圖片或純文字)
    if (e.Data.GetDataPresent(DataFormats.FileDrop))
    {
        // 如果是檔案，把滑鼠游標變成「複製」的圖示 (會有一個小 + 號)
        e.Effect = DragDropEffects.Copy;
    }
    else
    {
        // 如果不是檔案，顯示禁止圖示
        e.Effect = DragDropEffects.None;
    }
}

// 當滑鼠在文字框範圍「放開」時觸發
private void Txt_DragDrop(object sender, DragEventArgs e)
{
    // 取得拖放進來的檔案路徑清單 (因為可以一次選很多檔)
    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

    if (files != null && files.Length > 0)
    {
        // 抓取觸發這個事件的文字框 (是昨日還是今日？)
        TextBox targetBox = sender as TextBox;

        if (targetBox != null)
        {
            // 只取第一個檔案的路徑填入
            targetBox.Text = files[0];
            WriteLog($"使用者拖放載入檔案: {files[0]}"); // Log
        }
    }
}
// --- 拖放功能區域 End ---
    }
}