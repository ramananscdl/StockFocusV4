using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
	public class InputSavedEventArgs : EventArgs
	{
		public InputOperation Operation { get; set; }
		public List<GridItem> Data { get; set; }

	}

	public class ManageEventArgs : EventArgs
	{
		public object Data { get; set; }
		public ManageOperation Operation { get; set; }
		public Type UnderlyingObject { get; set; }

	}

	public class ManageEventArgs<T> : EventArgs
	{
		public T ObjValue { get; set; }
		public ManageOperation Operation { get; set; }
		public Type UnderlyingObject { get; set; }

	}

	public class SettingsEventArgs : EventArgs
	{
		public SettingBase  Settings { get; set; }

	}

}
