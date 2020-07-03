using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
	public class DisplayItem
	{
		public string PropertyName { get; set; }


		public List<KeyValuePair<int, string>> DataSource { get; set; }

		public string AlternateName { get; set; }
	}
}
