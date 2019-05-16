using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public class Symbol
    {
        public Exchange StockExchange { get; set; }

        public string Code { get; set; }
    }
}
