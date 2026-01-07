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
using ETF_Compare_2._0.Servcies;

namespace ETF_Compare_2._0
{
    public partial class Form1 : Form
    {

        // 宣告服務
        private readonly LogService _logger;
        private readonly ExcelService _excelService;
        private readonly AnalyzerService _analyzerService;
        public Form1()
        {
            InitializeComponent();


            // 初始化服務 (這是簡單的依賴注入概念)
            _logger = new LogService();
            _excelService = new ExcelService();
            _analyzerService = new AnalyzerService();


            // --- Log: 紀錄程式啟動 ---
            _logger.Write("程式啟動 - ETF Analyzer v1.0 Ready.");
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
                _logger.Write("警告: 昨日路徑無效或空白"); // Log
                return;
            }
            if (string.IsNullOrEmpty(pathToday) || !File.Exists(pathToday))
            {
                MessageBox.Show("請選擇有效的今日檔案路徑！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Write("警告: 今日路徑無效或空白"); // Log
                return;
            }

            try
            {
                _logger.Write("開始執行比對分析..."); // Log                
                btnCompare.Enabled = false; // 運算時禁用按鈕
                btnCompare.Text = "分析中...";

                // 使用 Task.Run 讓 Excel 讀取在背景執行，視窗才不會卡死
                var yesterdayData = await Task.Run(() => _excelService.ReadXlsx(pathYesterday));
                var todayData = await Task.Run(() => _excelService.ReadXlsx(pathToday));

                _logger.Write($"檔案讀取完成。昨日筆數: {yesterdayData.Count}, 今日筆數: {todayData.Count}"); // Log

                if (yesterdayData.Count == 0 || todayData.Count == 0)
                {
                    string errMsg = "讀取失敗，資料筆數為 0，請確認 Excel 內容。";
                    MessageBox.Show("讀取失敗，請確認 Excel 檔案內容是否包含「股票代號」！");
                    _logger.Write($"錯誤: {errMsg}"); // Log
                    return;
                }

                // 修正後的路徑設定(拿取 exe 同一層的資料夾路徑)
                string outputDir = Path.Combine(Application.StartupPath, "ETF_Output");
                DateTime targetDate = DateTime.Today; // 預設先抓今天 (如果檔名沒日期的話)
                string fileName = Path.GetFileNameWithoutExtension(pathToday); // 取得 "ETF_Portfolio_Composition_File_20260105"

                var match = System.Text.RegularExpressions.Regex.Match(fileName, @"20\d{6}");

                if (match.Success)
                {
                    string dateStr = match.Value; // 抓到 "20260105"
                    _logger.Write($"[系統] 檔名日期解析成功，基準日設定為: {targetDate:yyyy-MM-dd}");
                    // 嘗試將字串轉成真正的 DateTime 物件
                    if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        targetDate = parsedDate;
                        _logger.Write($"成功解析檔案日期，將查詢：{targetDate:yyyy-MM-dd} 的股價");
                    }
                }
                else
                {
                    _logger.Write("[系統警告] 檔名中找不到日期，將使用「今天」作為查詢基準。");
                }
                _logger.Write("開始比對並聯網抓價...");


                // 這段 lambda 語法定義了：當 Service 回報進度時，UI 要怎麼變
                var progressHandler = new Progress<(int current, int total, string message)>(status =>
                {
                    progressBar1.Maximum = status.total;  // 設定最大值
                    progressBar1.Value = status.current;  // 設定目前進度
                    lblStatus.Text = status.message;      // 讓 Label 顯示「正在分析: 台積電...」
                });
                // 7. 彈出 Yes/No 選擇視窗
                // MessageBoxButtons.YesNo 會顯示「是」與「否」按鈕
                // MessageBoxIcon.Question 會顯示問號圖示
               
               
                var result = await _analyzerService.CompareAndGenerateReport(yesterdayData, todayData, pathYesterday, pathToday, outputDir, targetDate, progressHandler);

                _logger.Write($"比對完成，變動數: {result.ChangeCount}");
                // 先暫停 0.1 秒，讓那個「遲到的最後一筆進度更新」先跑完
                // 這樣它才不會覆蓋掉我們後面要寫的「分析完畢」
                await Task.Delay(100);

                // 現在確認沒有遲到的訊號了，我們設定最終狀態
                lblStatus.Text = "分析完畢";
                progressBar1.Value = 0;    // 清空進度條

                //強制立刻重畫 (確保 MessageBox 跳出前，使用者看到的是乾淨的畫面)
                lblStatus.Refresh();
                progressBar1.Refresh();

                // 再給一點點時間確保繪圖完成
                await Task.Delay(50);

                // 8. 判斷使用者的選擇
                string msg = $"比對完成！\n發現 {result.ChangeCount} 筆變動。\n報告存於：\n{result.OutputPath}\n\n是否開啟資料夾？";
               if (MessageBox.Show(msg, "成功", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                        System.Diagnostics.Process.Start("explorer.exe", outputDir);
                }
               
                // 如果選「否」，程式就會直接往下執行 (也就是不做任何事)，不會打開資料夾
            }
            catch (Exception ex)
            {
                _logger.Write($"[嚴重錯誤 Exception] {ex.Message}\n堆疊: {ex.StackTrace}");
                MessageBox.Show($"執行發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "發生錯誤"; // 錯誤時顯示
            }
            finally
            {
                btnCompare.Enabled = true;
                btnCompare.Text = "比對";
                txtYesterday.Text = "";
                txtToday.Text = "";
                _logger.Write("作業結束，重置介面。"); // Log
                progressBar1.Value = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtYesterday.Text = ""; // 清空文字
                                    // txtYesterday.Clear(); // 用這行也可以，效果一樣
            txtYesterday.Focus();  // 讓游標跳回格子裡閃爍
            _logger.Write("使用者手動清除昨日路徑"); // Log
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtToday.Text = ""; // 清空文字
            txtToday.Focus();  // 讓游標跳回格子裡閃爍
            _logger.Write("使用者手動清除今日路徑"); // Log
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
            _logger.Write($"使用者拖放載入檔案: {files[0]}"); // Log
        }
    }
}

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }


        // --- 拖放功能區域 End ---
    }
}