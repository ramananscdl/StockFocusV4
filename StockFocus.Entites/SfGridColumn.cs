using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public class SfGridColumn
    {
        public SfGridColumn()
        {

        }
        public SfGridColumn(string name)
        {
            ColumnName = name;
            DisplayName = name;
        }
        public SfGridColumn(string columnname, string displayName)
        {
            ColumnName = columnname;
            DisplayName = displayName;
        }
        public string DisplayName { get; set; }
        public string ColumnName { get; set; }
    }
}
