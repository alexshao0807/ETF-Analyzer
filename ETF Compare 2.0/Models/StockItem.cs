using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETF_Compare_2._0.Models
{
        // 定義資料結構
        public class StockItem
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public decimal Shares { get; set; }
            public string Weight { get; set; }

            public decimal Price { get; set; }
        }

    
}
