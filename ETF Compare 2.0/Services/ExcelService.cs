using ClosedXML.Excel;
using ETF_Compare_2._0.Models; // 記得引用 Models
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// --- 4. 讀取 Excel 方法 ---
namespace ETF_Compare_2._0.Servcies
{
    public  class ExcelService
    {
        public List<StockItem> ReadXlsx(string filePath)
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
    }
}
