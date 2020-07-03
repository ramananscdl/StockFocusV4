using StockFocus.Entites;
using StockFocus.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StockFocus.UI
{
	public class SFTextBox : TextBox, IsfControls
	{
		public int TransactionId { get; set; }
		public TransactionColumn Field { get; set; }
	}
	public class SFNTextBox : NumericTextBox, IsfControls
	{
		public int TransactionId { get; set; }
		public TransactionColumn Field { get; set; }
	}

	public class SFComboBox : ComboBox, IsfControls
	{
		public int TransactionId { get; set; }
		public TransactionColumn Field { get; set; }
	}

	public class SFDatePicket : DatePicker, IsfControls
	{
		public int TransactionId { get; set; }
		public TransactionColumn Field { get; set; }
	}
	public class SFListBoxItem : ListBoxItem 
	{
		public string Text { get; set; }
	}

	public static class Extensions
	{
		public static List<SfGridColumn> GetGridColumns(this ListBox lb)
		{
			List<SfGridColumn> gc = new List<SfGridColumn>();
			foreach (var item in lb.Items)
			{
				var lbi = item as SFListBoxItem;
				gc.Add(new SfGridColumn(lbi.Tag.ToString(), lbi.Text));
			}  

			return gc;

		}
		public static object Find(this ItemCollection ic, Predicate<object> pred)
		{
			 
			foreach (var item in ic)
			{
				if(pred.Invoke(item))
				{
					return item;
				}
				 
			}

			return null;

		}
	}

}
