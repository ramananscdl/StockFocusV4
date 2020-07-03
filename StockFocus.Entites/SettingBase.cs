using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
	public class SettingBase
	{
		public string DBConnectionString { get; set; }
		public int RefreshInterval { get; set; }

		public List<SfGridColumn> DisplayColumns { get; set; }

		public List<string> UploadColumns { get; set; }



		
		public SettingBase()
		{
			
		}
	}


	
}
