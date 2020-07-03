using StockFocus.Entites;
using StockFocus.Helper;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFocus.UI
{
	/// <summary>
	/// Interaction logic for ManageForm.xaml
	/// </summary>
	public partial class ManageForm : UserControl
	{
		public ManageForm()
		{
			InitializeComponent();
		}


		public delegate void delgSaveClicked(object sender, ManageEventArgs Data);
		public delegate void delgDeleteClicked(object sender, ManageEventArgs Data);
		public event delgSaveClicked SaveClicked;
		public event delgDeleteClicked DeleteClicked;
		public event EventHandler CancelClicked;

		IEnumerable<string> displayColumns = new List<string>();
		public IEnumerable<string> DisplayColumns { get => displayColumns; set => displayColumns = value; }
		public string KeyColumn { get; set; }

		public List<DisplayItem> DisplayItems { get; set; }

		public string ValueColumn { get; set; }
		private OperationMode mode;
		public object ItemKey { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		object ActiveItem = null;

		public List<KeyValuePair<string, int>> ColumnMaxSize { get; set; }


		public void DataBind<T>(List<T> DataItem)
		{
			grdTopic.Children.Clear();
			grdText.Children.Clear();
			grdControl.Children.Clear();
			UnderlyingType = typeof(T);
			mode = OperationMode.Multiple;
			txtTitle.Content = Title;
			lblDescription.Content = Description;

			this.Height = 300.00;
			if (DataItem != null)
			{



				RowDefinition rd = new RowDefinition() { Height = new GridLength(35) };
				ComboBox cbx = new ComboBox() { FontSize = 15, Height = 30 };
				cbx.SelectionChanged += Cbx_SelectionChanged;
				DataItem.ForEach(di =>
				{
					cbx.Items.Add(di);

				});
				cbx.DisplayMemberPath = ValueColumn;
				cbx.SelectedValuePath = KeyColumn;
				grdTopic.RowDefinitions.Add(rd);
				Grid.SetRow(cbx, 0);
				grdTopic.Children.Add(cbx);





			}
		}

		public void DataBind<T>(T DataItem)
		{
			grdTopic.Children.Clear();
			grdText.Children.Clear();
			grdControl.Children.Clear();

			mode = OperationMode.Single;
			ItemKey = DataItem.GetType().GetProperty(KeyColumn).GetValue(DataItem);
			this.Description = DataItem.GetType().GetProperty(ValueColumn).GetValue(DataItem).ToString();
			UnderlyingType = typeof(T);
			txtTitle.Content = Title;
			lblDescription.Content = Description;
			BindItem(DataItem);

		}

		public Type UnderlyingType { get; set; }



		private void Cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var sItem = ((ComboBox)sender).SelectedItem;
			BindItem(sItem);

		}



		private void BindItem(object sItem)
		{
			ActiveItem = sItem;
			grdControl.Children.Clear();
			int count = 0;
			this.Height = 220;
			sItem.GetType().GetProperties().ToList().ForEach(pi =>
			{
				if (DisplayColumns.Any(s => s == pi.Name) && pi.CanWrite)
				{
					RowDefinition rdLabel = new RowDefinition() { Height = new GridLength(35) };

					grdText.RowDefinitions.Add(rdLabel);
					Label lbl = new Label() { FontSize = 15, Height = 30, Foreground = Brushes.Teal, HorizontalContentAlignment = HorizontalAlignment.Right };
					Grid.SetRow(lbl, count);
					lbl.Content = pi.Name;
					grdText.Children.Add(lbl);
					RowDefinition rdContent = new RowDefinition() { Height = new GridLength(35) };
					grdControl.RowDefinitions.Add(rdContent);
					this.Height += 32;

					if (pi.PropertyType == typeof(DateTime))
					{
						DatePicker dp = new DatePicker() { FontSize = 15, Height = 30, SelectedDate = (DateTime)pi.GetValue(sItem) };
						Grid.SetRow(dp, count);
						dp.Tag = pi.Name;
						grdControl.Children.Add(dp);

					}
					else if (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(double))
					{
						NumericTextBox ntb = new NumericTextBox() { FontSize = 15, DecimalNumbers = 2, Text = pi.GetValue(sItem).ToString(), Height = 30 };
						Grid.SetRow(ntb, count);
						ntb.Tag = pi.Name;
						grdControl.Children.Add(ntb);
					}
					else if (pi.PropertyType == typeof(bool))
					{
						CheckBox cb = new CheckBox() { Height = 30 };
						Grid.SetRow(cb, count);
						cb.Tag = pi.Name;
						cb.IsChecked = (bool)pi.GetValue(sItem);
						grdControl.Children.Add(cb);
					}
					else if (pi.PropertyType.IsEnum)
					{
						ComboBox cb = new ComboBox() { FontSize = 15, Height = 30 };
						cb.Tag = pi.Name;
						cb.ItemsSource = Enum.GetValues(pi.PropertyType);
						cb.SelectedValue = pi.GetValue(sItem);
						Grid.SetRow(cb, count);
						grdControl.Children.Add(cb);
					}
					else
					{
						if (DisplayItems != null && DisplayItems.Any(di => di.PropertyName == pi.Name))
						{
							ComboBox cb = new ComboBox() { FontSize = 15, Height = 30 };
							cb.Tag = pi.Name;
							cb.IsReadOnly = pi.Name == KeyColumn;
							cb.ItemsSource = DisplayItems.Find(d => d.PropertyName == pi.Name).DataSource;
							cb.DisplayMemberPath = "Value";
							cb.SelectedValuePath = "Key";
							cb.SelectedValue = pi.GetValue(sItem);
							Grid.SetRow(cb, count);
							grdControl.Children.Add(cb);
						}
						else
						{
							TextBox tb = new TextBox() { FontSize = 15, Height = 30 };

							tb.Tag = pi.Name;
							if (pi.Name == KeyColumn)
							{
								tb.IsReadOnly = true;
								tb.BorderThickness = new Thickness(0.00);
								tb.Background = this.Background;
								
							}
							if (ColumnMaxSize != null && ColumnMaxSize.Any(kv => kv.Key == pi.Name))
							{
								tb.MaxLength = ColumnMaxSize.Find(kv => kv.Key == pi.Name).Value;
							}
							tb.Text = pi.GetValue(sItem).ToString();
							Grid.SetRow(tb, count);
							grdControl.Children.Add(tb);

						}
					}
					count++;
				}

			});
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Height = 220;
			CancelClicked?.Invoke(this, e);
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			var result = Convert.ChangeType(ActiveItem, UnderlyingType);
			foreach (var child in grdControl.Children)
			{
				var propertyName = ((Control)child).Tag;
				if (propertyName != null)
				{

					try
					{
						UnderlyingType.GetProperty(propertyName.ToString()).SetValue(result, GetControlValue(child));
					}
					catch (Exception ex)
					{
						string s = ex.Message;
					}

				}

			}

			if (mode == OperationMode.Multiple)
				ItemKey = (grdTopic.Children.FindFirst<ComboBox>(c => c != null)).SelectedValue;


			UnderlyingType.GetProperty(KeyColumn).SetValue(result, ItemKey);
			SaveClicked?.Invoke(this, new ManageEventArgs() { Data = result, Operation = ManageOperation.Edit, UnderlyingObject = UnderlyingType });
		}

		private object GetControlValue(object child)
		{
			decimal d = 0;
			int ival = 0;
			object result = null;



			if (child is DatePicker)
				result = ((DatePicker)child).SelectedDate;
			else if (child is CheckBox)
				result = (bool)((CheckBox)child).IsChecked;
			else if (child is ComboBox)
				result = ((ComboBox)child).SelectedValue;
			else if (child is TextBox)
			{
				string s = ((TextBox)child).Text;

				//	return Convert.ChangeType(s, typeof(int));

				if (int.TryParse(s, out ival))
					return ival;
				else if (decimal.TryParse(s, out d))
					return d;
				else return s;
			}


			return result;
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("This will delete all the record and its Children records. Do you still want to delete?", "Deleting...", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				var result = Activator.CreateInstance(UnderlyingType);
				var keyVal = (grdTopic.Children.FindFirst<ComboBox>(c => c != null)).SelectedValue;
				UnderlyingType.GetProperty(KeyColumn).SetValue(result, keyVal);
				DeleteClicked?.Invoke(this, new ManageEventArgs() { Data = result, Operation = ManageOperation.Delete, UnderlyingObject = UnderlyingType });
			}
		}
	}
}
