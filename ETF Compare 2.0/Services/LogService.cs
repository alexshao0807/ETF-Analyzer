using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ETF_Compare_2._0.Servcies
{// --- ★ 新增功能: 寫入 Log 檔案的方法 ---
    public class LogService
    {
        private readonly string _logDir;
        public LogService()
        {
            // 1. 設定 Logs 資料夾路徑 (在 exe 旁邊)
            _logDir = Path.Combine(Application.StartupPath, "Logs");
            if (!Directory.Exists(_logDir)) Directory.CreateDirectory(_logDir);
        }
        public void Write(string message)
        {
            try
            {
                // 2. 設定檔名 (每天一個檔案，例如 Log_20231025.txt)
                string logPath = Path.Combine(_logDir, $"Log_{DateTime.Now:yyyyMMdd}.txt");

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
       
    }
}
