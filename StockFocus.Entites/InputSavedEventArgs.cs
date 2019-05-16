using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public class InputSavedEventArgs:EventArgs
    {
        public InputOperation  Operation { get; set; }
        public List<GridItem> Data { get; set; }
        
    }
    
}
