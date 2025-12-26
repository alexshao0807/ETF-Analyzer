using ETF_Compare_2._0.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETF_Compare_2._0.Servcies
{
    
    

    // 定義一個回傳結果的類別，方便 Form1 拿資料
    public class AnalysisResult
    {
        public string ReportContent { get; set; }
        public int ChangeCount { get; set; }
        public string OutputPath { get; set; }
    }


    public class AnalyzerService
    {
        private readonly LogService _logger = new LogService();


        public AnalysisResult CompareAndGenerateReport(List<StockItem> yesterdayData, List<StockItem> todayData, string pathYesterday, string pathToday, string outputDir)
        {

            
            // $O(1) 比對：建立昨日字典
            var yesterdayDict = yesterdayData.ToDictionary(s => s.Symbol);
            StringBuilder sb = new StringBuilder();
            // 1. 產生報告標頭
            sb.AppendLine($"ETF 持股變動報告 - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"昨日檔案：{Path.GetFileName(pathYesterday)}");
            sb.AppendLine($"今日檔案：{Path.GetFileName(pathToday)}");
            sb.AppendLine(new string('-', 75));
            sb.AppendLine($"{"狀態",-6} {"代號",-10} {"名稱",-15} {"今日股數",15} {"張數變動",15}");

            // 2. 核心比對
            int changeCount = 0;
            foreach (var current in todayData)
            {
                if (yesterdayDict.TryGetValue(current.Symbol, out var prev))
                {
                    decimal diff = current.Shares - prev.Shares;

                    if (diff != 0)
                    {
                        string status = diff > 0 ? "增加" : "減少";

                        sb.AppendLine($"{status,-6} {current.Symbol,-10} {current.Name,-15} {current.Shares,20:N0} {diff,20:N0}");
                        changeCount++;
                    }
                }
                else
                {
                    sb.AppendLine($"新進   {current.Symbol,-10} {current.Name,-15} {current.Shares,15:N0} {current.Shares,15:N0}");
                    changeCount++;
                }
            }

            _logger.Write($"比對邏輯完成，發現 {changeCount} 筆變動。"); // Log
                                                      
                                                      

            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            // 設定完整的檔案路徑
            string outputPath = Path.Combine(outputDir, $"Analysis_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            // 4. 補充文字檔結尾訊息
            sb.AppendLine(new string('-', 75));
            sb.AppendLine($"比對完成！發現 {changeCount} 筆變動。");
            sb.AppendLine($"報告存於：{outputPath}");
            // 5. 寫入檔案
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            _logger.Write($"報告已存檔: {outputPath}"); // Log
            // 6. 準備訊息內容 (合併 成功通知 + 詢問)
            string message = $"比對完成！\n" +
                             $"發現 {changeCount} 筆變動。\n\n" +
                             $"報告已儲存於：\n{outputPath}\n\n" +
                             $"請問是否要立即開啟輸出資料夾？";

            return new AnalysisResult
            {
                ReportContent = sb.ToString(),
                ChangeCount = changeCount,
                OutputPath = outputPath
            };
        }
    }
}
