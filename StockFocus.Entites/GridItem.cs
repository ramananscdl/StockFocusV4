using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
	public class GridItem
	{
		public string ItemName { get; set; }

		public string DisplayName { get; set; }

		public InputType InputControl { get; set; }

		public int DecimalNumbers { get; set; }

		public List<KeyValuePair<int, string>> ControlValues { get; set; }

		public dynamic ReturnValue { get; set; }

		public bool IsMandatory
		{
			get
			{
				return isMandatory;
			}

			set
			{
				isMandatory = value;
			}
		}

		bool isMandatory = true;



	}

	//public class GridDefItem<T> : GridItem
	//{
	//	public T Default { get; set; }
	//}
}
