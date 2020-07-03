using StockFocus.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFocus.UI
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : UserControl
	{
		public SettingsWindow()
		{
			 
			InitializeComponent();
			
			LoadAvailableColumns();
		 

		}

		public delegate void delegSaveClicked (object sender, SettingsEventArgs data); 
		public event delegSaveClicked SaveClicked;
		public event EventHandler CancelClicked;

		private void LoadSettingsValues()
		{

			 
			if (BaseSettings.DisplayColumns == null || BaseSettings.DisplayColumns.Count < 2)
			{
				ExistingDisplayColumns.Add(new SfGridColumn("StockName", "Stock Name"));
				ExistingDisplayColumns.Add(new SfGridColumn("CMP", "Last Price"));
				ExistingDisplayColumns.Add(new SfGridColumn("DifferenceInPercentage", "Change %"));
				ExistingDisplayColumns.Add(new SfGridColumn("Change", "Change"));
				ExistingDisplayColumns.Add(new SfGridColumn("Quantity", "Quantity"));
				ExistingDisplayColumns.Add(new SfGridColumn("CurrentValue", "Current Value"));
				ExistingDisplayColumns.Add(new SfGridColumn("InvestedValue", "Invested Value"));
				ExistingDisplayColumns.Add(new SfGridColumn("NetProfit", "Gain / Loss"));
				ExistingDisplayColumns.Add(new SfGridColumn("AdjustedPrice", "Break Even"));
				ExistingDisplayColumns.Add(new SfGridColumn("InvestedWeightage", "Inv - Wtg"));
				ExistingDisplayColumns.Add(new SfGridColumn("CurrentWeightage", "Cur - Wtg"));
				BaseSettings.DisplayColumns = ExistingDisplayColumns;
			}
			BaseSettings.RefreshInterval = BaseSettings.RefreshInterval == 0 ? 15 : BaseSettings.RefreshInterval;

			txtDBConnection.Text = BaseSettings.DBConnectionString;
			lbRefreshInterval.SelectedItem = lbRefreshInterval.Items.Find(it => ((TextBlock)it).Tag.ToString() == BaseSettings.RefreshInterval.ToString());
		 
			ExistingDisplayColumns = BaseSettings.DisplayColumns;

		}
		SettingBase baseSettings = new SettingBase();
		public SettingBase BaseSettings
		{
			get
			{
				return baseSettings;
			}
			set
			{
				baseSettings = value;
				LoadSettingsValues();
				PopulateColumnsinList();

			}
		}
		private void LoadAvailableColumns()
		{
			AllColumns.Clear();
			AllColumns.Add(new SfGridColumn("StockName", "Stock Name"));
			AllColumns.Add(new SfGridColumn("DifferenceInPercentage", "Change %"));
			AllColumns.Add(new SfGridColumn("CMP", "Last Price"));
			AllColumns.Add(new SfGridColumn("Change", "Change"));
			AllColumns.Add(new SfGridColumn("Quantity", "Quantity"));
			AllColumns.Add(new SfGridColumn("Close", "Previous Close"));
			AllColumns.Add(new SfGridColumn("CurrentValue", "Current Value"));
			AllColumns.Add(new SfGridColumn("InvestedValue", "Invested Value"));
			AllColumns.Add(new SfGridColumn("NetProfit", "Gain / Loss"));
			AllColumns.Add(new SfGridColumn("AdjustedPrice", "Break Even"));
			AllColumns.Add(new SfGridColumn("DayLow", "Day Low"));
			AllColumns.Add(new SfGridColumn("DayHigh", "Day High"));
			AllColumns.Add(new SfGridColumn("InvestedWeightage", "Inv - Wtg"));
			AllColumns.Add(new SfGridColumn("CurrentWeightage", "Cur - Wtg"));
			AllColumns.Add(new SfGridColumn("PurchasePrice", "Purchase Price"));
		 
		}

		private void PopulateColumnsinList()
		{
			lstAvlColumn.Items.Clear();
			lstDispColumn.Items.Clear();
			AvailableColumns.Clear();
			 
			AllColumns.ForEach(ac =>
			{
				if (!ExistingDisplayColumns.Any(ed => ed.ColumnName == ac.ColumnName))
				{
					AvailableColumns.Add(new SfGridColumn(ac.ColumnName, ac.DisplayName));
				}
			});

			AvailableColumns.ForEach(ac => lstAvlColumn.Items.Add(AddLBItem(ac)));
			ExistingDisplayColumns.ForEach(ed => lstDispColumn.Items.Add(AddLBItem(ed)));
			lstAvlColumn.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Text", System.ComponentModel.ListSortDirection.Ascending));

		}

		ListBoxItem AddLBItem(SfGridColumn column)
		{
			TextBlock txt = new TextBlock() { Text = column.DisplayName, Style = (Style)Application.Current.FindResource("ListItemText") };
			SFListBoxItem lbi = new SFListBoxItem();
			lbi.Content = txt;
			lbi.Text = column.DisplayName;
			lbi.Tag = column.ColumnName;
			return lbi;
		}


		static List<SfGridColumn> AvailableColumns = new List<SfGridColumn>();
		static List<SfGridColumn> ExistingDisplayColumns = new List<SfGridColumn>();
		static List<SfGridColumn> AllColumns = new List<SfGridColumn>();




		private void btnLtR_Click(object sender, RoutedEventArgs e)
		{
			List<ListBoxItem> lbis = new List<ListBoxItem>();
			foreach (var item in lstAvlColumn.SelectedItems)
			{
				lbis.Add((ListBoxItem)item);

			}
			if (lstDispColumn.Items.Count > 11)
			{
				MessageBox.Show("You cannot add more than 12 columns", "Exceeds Column Count", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			lbis.ForEach(l =>
			{
				lstAvlColumn.Items.Remove(l);
				lstDispColumn.Items.Add(l);
			});
		}

		private void btnRtL_Click(object sender, RoutedEventArgs e)
		{
			List<ListBoxItem> lbis = new List<ListBoxItem>();
			foreach (var item in lstDispColumn.SelectedItems)
			{
				lbis.Add((ListBoxItem)item);

			}
			if (lbis.Any(lb => lb.Tag.ToString().Trim() == "StockName"))
			{
				MessageBox.Show("You cannot remove 'Stock Name' column", "Mandatory Column", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			lbis.ForEach(l =>
			{
				lstDispColumn.Items.Remove(l);
				lstAvlColumn.Items.Add(l);
				//lstAvlColumn.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Text", System.ComponentModel.ListSortDirection.Ascending));
			});

			lstAvlColumn.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Text", System.ComponentModel.ListSortDirection.Ascending));

		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			SettingBase sb = new SettingBase();
			sb.DBConnectionString = txtDBConnection.Text;
			sb.DisplayColumns = lstDispColumn.GetGridColumns();
			sb.RefreshInterval = int.Parse(((TextBlock)lbRefreshInterval.SelectedItem).Tag.ToString());
			SaveClicked?.Invoke(this, new SettingsEventArgs() { Settings = sb });
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			CancelClicked?.Invoke(this, null);
		}

		 
	}
}
