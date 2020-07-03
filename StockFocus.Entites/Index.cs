using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public class Index
    {
        public string IndexName { get; set; }

        public decimal CMP { get; set; }

        public decimal ChangePercentage { get; set; }

        public decimal Change { get; set; }
    }
}
