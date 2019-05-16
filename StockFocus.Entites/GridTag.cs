using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public class GridTag
    {
        public GridTag(string columnName, GridSortOrder order)
        {
            ColumnName = columnName;
            Order = order;
        }
        public GridTag()
        {

        }
        public string ColumnName { get; set; }

        public GridSortOrder Order { get; set; }

    }
}
