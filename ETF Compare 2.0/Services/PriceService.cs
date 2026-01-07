using Newtonsoft.Json.Linq; // 剛剛安裝的套件就是為了這行
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ETF_Compare_2._0.Servcies
{
    public class PriceService
    {
        // 建立一個靜態的瀏覽器物件 (HttpClient)，整個程式共用這一個，效率最高
        private static readonly HttpClient _client = new HttpClient();

        private readonly LogService _logger = new LogService();
        public PriceService()
        {
            // 設定 User-Agent，讓 Yahoo 覺得我們是正常的 Chrome 瀏覽器
            if (_client.DefaultRequestHeaders.UserAgent.Count == 0)
            {
                _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            }
        }

        // --- 核心功能：傳入股票代號，回傳現在的價格 ---
        public async Task<decimal> GetStockPriceAsync(string symbol, DateTime date)
        {
            // 1. 計算時間區間
            long startTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds();
            long endTimestamp = new DateTimeOffset(date.AddDays(1)).ToUnixTimeSeconds();

            // 定義一個變數來存最後抓到的 JSON
            string jsonResponse = null;
            string targetSymbol = "";

            try
            {
                // --- 第一次嘗試：上市 (.TW) ---
                targetSymbol = symbol.Trim() + ".TW";
                string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{targetSymbol}?period1={startTimestamp}&period2={endTimestamp}&interval=1d";
                _logger.Write($"[爬蟲] 嘗試抓取上市代號: {targetSymbol}"); // (可選) 紀錄嘗試中
                // 這裡如果失敗 (404)，會直接跳到下面的 catch
                jsonResponse = await _client.GetStringAsync(url);
            }
            catch
            {
                _logger.Write($"[爬蟲警告] 抓取 {targetSymbol} 失敗 (可能是 404)，準備嘗試上櫃代碼...");
                // 第一次失敗了 (可能是 404 找不到)，這裡「吃掉」錯誤，準備嘗試第二次
            }

            // 如果第一次沒拿到資料，嘗試第二次
            if (string.IsNullOrEmpty(jsonResponse))
            {
                try
                {
                    // --- 第二次嘗試：上櫃 (.TWO) ---
                    targetSymbol = symbol.Trim() + ".TWO";
                    string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{targetSymbol}?period1={startTimestamp}&period2={endTimestamp}&interval=1d";
                    _logger.Write($"[爬蟲重試] 改抓取上櫃代號: {targetSymbol}");
                    jsonResponse = await _client.GetStringAsync(url);
                }
                catch
                {
                    _logger.Write($"[爬蟲錯誤] {symbol} 最終抓取失敗 (上市/上櫃皆無回應)。");
                    // 如果連上櫃都失敗，那就是真的失敗了，回傳 0
                    return 0;
                }
            }

            // --- 解析資料 (無論是第一次還是第二次成功的) ---
            try
            {
                if (!string.IsNullOrEmpty(jsonResponse))
                {
                    JObject data = JObject.Parse(jsonResponse);

                    // 檢查是否真的有資料
                    if (data["chart"]["result"]?.Type != JTokenType.Null)
                    {
                        var result = data["chart"]["result"]?[0];
                        var closePrice = result?["indicators"]?["quote"]?[0]?["close"]?[0];

                        if (closePrice != null && closePrice.Type != JTokenType.Null)
                        {
                            decimal price = closePrice.Value<decimal>();
                            _logger.Write($"[爬蟲成功] {targetSymbol} 在 {date:yyyy-MM-dd} 收盤價: {price}");
                            return price;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Write($"[解析錯誤] 解析 {symbol} JSON 時發生例外: {ex.Message}");
                // 解析發生未預期錯誤
            }

            return 0;
        }
    }
}