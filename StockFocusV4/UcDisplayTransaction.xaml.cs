using StockFocus.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFocus.UI
{
	/// <summary>
	/// Interaction logic for UcDisplayTransaction.xaml
	/// </summary>
	public partial class UcDisplayTransaction : UserControl
	{
		public UcDisplayTransaction()
		{
			InitializeComponent();


		}
		public delegate void delgUcButtonClicked(object sender, ManageEventArgs<Transaction> Data);
		public event delgUcButtonClicked SaveClicked;
		public event delgUcButtonClicked DeleteClicked;



		public Stock Stock { get; set; }

		public int PortfolioId { get; set; }


		public List<Transaction> Transactions { get; set; }



		public void DataBind()
		{
			string[] cbxArray = { "Buy", "Sell" };
			MainGrid.Children.Clear(u => u is TextBox);
			this.Height = 50;
			this.txtStockName.Content = Stock.StockName;
			txtRate.Content = "Avg.Price:" + Stock.AdjustedPrice.ToString(" ₹ 0.00");
			txtQty.Content = "Qty:" + Stock.Quantity.ToString();

			int count = 1;
			foreach (var tr in Transactions)
			{
				count++;
				RowDefinition rd = new RowDefinition() { Height = new GridLength(35) };
				MainGrid.RowDefinitions.Add(rd);

				SFTextBox tbDate = new SFTextBox() { Text = tr.TransactionDate.ToString("dd-MMM-yyyy"), HorizontalAlignment = HorizontalAlignment.Center, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), TransactionId = tr.TransactionId, Field = TransactionColumn.Date };
				SFNTextBox tbAmount = new SFNTextBox() { Text = tr.Amount.ToString("0.00"), HorizontalAlignment = HorizontalAlignment.Right, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), TransactionId = tr.TransactionId, Field = TransactionColumn.Amount, DecimalNumbers = 2 };
				SFNTextBox tbQty = new SFNTextBox() { Text = tr.Quantity.ToString(), HorizontalAlignment = HorizontalAlignment.Right, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), TransactionId = tr.TransactionId, Field = TransactionColumn.Quantity, DecimalNumbers = 0 };
				SFNTextBox tbTotal = new SFNTextBox() { Text = (tr.Quantity * tr.Amount).ToString("0.00"), HorizontalAlignment = HorizontalAlignment.Right, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), TransactionId = tr.TransactionId, Field = TransactionColumn.TotalAmount, DecimalNumbers = 2, };
				SFTextBox tbTransaction = new SFTextBox() { Text = tr.TransactionType.ToString(), HorizontalAlignment = HorizontalAlignment.Right, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), TransactionId = tr.TransactionId, Field = TransactionColumn.Transaction };

				



				Button btnEdit = new Button() { Content = "  Edit  ", Height = 30, Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader"), Tag = tr.TransactionId, Background = (Brush)Application.Current.FindResource("ButtonBG"), Foreground = Brushes.Beige };
				Button btnDelete = new Button() { Content = " Delete ", Height = 30, Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader"), Tag = tr.TransactionId, Background = (Brush)Application.Current.FindResource("ButtonBG"), Foreground = Brushes.Beige };
				Button btnCancel = new Button() { Content = " Cancel ", Height = 30, Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader"), Visibility = Visibility.Hidden, Tag = tr.TransactionId, Background = (Brush)Application.Current.FindResource("ButtonBG"), Foreground = Brushes.Beige };
				SFComboBox cbx = new SFComboBox() { Height = 30, ItemsSource = cbxArray, Visibility = Visibility.Hidden, TransactionId = tr.TransactionId, Field = TransactionColumn.Transaction };
				cbx.SelectedValue = (tr.TransactionId == 1 ? "Buy" : "Sell");

				btnCancel.Click += BtnCancel_Click;

				btnEdit.Click += BtnEdit_Click;

				btnDelete.Click += BtnEdit_Click;
				tbAmount.LostFocus += TbAmount_LostFocus;
				tbQty.LostFocus += TbAmount_LostFocus;

				Brush cvBg = Background = Brushes.Wheat;

				Canvas cvDate = new Canvas() { Background = cvBg }; cvDate.Children.Add(tbDate);
				Canvas cvAmount = new Canvas() { Background = cvBg }; cvAmount.Children.Add(tbAmount);
				Canvas cvQty = new Canvas() { Background = cvBg }; cvQty.Children.Add(tbQty);
				Canvas cvTotal = new Canvas() { Background = cvBg }; cvTotal.Children.Add(tbTotal);

				Canvas cvBtns = new Canvas() { Background = cvBg };
				Canvas cvTrans = new Canvas() { Background = cvBg }; cvTrans.Children.Add(tbTransaction);

				cvBtns.Children.Add(btnEdit); cvBtns.Children.Add(btnCancel); cvTrans.Children.Add(cbx); cvBtns.Children.Add(btnDelete);

				Canvas.SetLeft(btnEdit, 10);
				Canvas.SetLeft(btnDelete, 120);
				Canvas.SetLeft(btnCancel, 60);

				Grid.SetColumn(cvDate, 0);
				Grid.SetColumn(cvAmount, 2);
				Grid.SetColumn(cvTotal, 3);
				Grid.SetColumn(cvBtns, 5);
				Grid.SetColumn(cvTrans, 4);


				Grid.SetColumn(cvQty, 1);

				Grid.SetRow(cvDate, count);
				Grid.SetRow(cvAmount, count);
				Grid.SetRow(cvTotal, count);
				Grid.SetRow(cvQty, count);

				Grid.SetRow(cvBtns, count);
				Grid.SetRow(cvTrans, count);
				this.Height += 40;

				MainGrid.Children.Add(cvDate);
				MainGrid.Children.Add(cvAmount);
				MainGrid.Children.Add(cvTotal);
				MainGrid.Children.Add(cvQty);
				MainGrid.Children.Add(cvBtns);

				MainGrid.Children.Add(cvTrans);
			}
		}

		private void TbAmount_LostFocus(object sender, RoutedEventArgs e)
		{
			int transactionId = ((SFNTextBox)sender).TransactionId;
			MainGrid.Children.FindAllLevelChildrenByType<SFTextBox>(t => { return t.TransactionId == transactionId && t.Field == TransactionColumn.TotalAmount; });
		}

		private void BtnEdit_Click(object sender, RoutedEventArgs e)
		{
			int transactionId = (int)((Button)sender).Tag;
			Transaction trn = Transactions.Find(t => t.TransactionId == transactionId);
			var cancelButton = (((Button)sender).Parent as Canvas).Children.FindFirst<Button>(IsCancelButton);
			string process = ((Button)sender).Content.ToString();
			if (process.Trim() == "Edit")
			{
				Transactions.ForEach(tr => ModifyUnderlyingTransaction(tr.TransactionId));
				((Button)sender).Content = " Save ";
				cancelButton.Visibility = Visibility.Visible;
				int row = (int)cancelButton.Tag;

				MainGrid.Children.FindAllLevelChildrenByType<SFTextBox>(t => t.TransactionId == trn.TransactionId).ForEach(t =>
			   {
				   t.BorderThickness = new Thickness(1);
				   t.Background = Brushes.White;
				   t.IsReadOnly = false;
			   });
				MainGrid.Children.FindAllLevelChildrenByType<SFNTextBox>(t => t.TransactionId == trn.TransactionId && t.Field !=  TransactionColumn.TotalAmount).ForEach(t =>
				{
					t.BorderThickness = new Thickness(1);
					t.Background = Brushes.White;
					t.IsReadOnly = false;
				});
				MainGrid.Children.FindAllLevelChildrenByType<SFComboBox>(c => (c as SFComboBox).TransactionId == trn.TransactionId).ForEach
					(cb => { cb.Visibility = Visibility.Visible; cb.SelectedValue = trn.TransactionType.ToString(); });



			}
			else if (process.Trim() == "Save")
			{
				ModifyUnderlyingTransaction(transactionId, true);
				SaveClicked(this, new ManageEventArgs<Transaction>() { ObjValue = Transactions.Find(t => t.TransactionId == transactionId), Operation = ManageOperation.Edit, UnderlyingObject = typeof(Transaction) });
			}
			else if (process.Trim() == "Delete")
			{
				if(MessageBox.Show("Do you want to delete this transaction?" , "Deleting Transaction ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					
					DeleteClicked(this, new ManageEventArgs<Transaction>() { ObjValue = Transactions.Find(t => t.TransactionId == transactionId), Operation = ManageOperation.Delete, UnderlyingObject = typeof(Transaction) });
					Transactions.RemoveAll(tr => tr.TransactionId == transactionId);
					DataBind();
				}
			}
			else
			{
				((Button)sender).Content = " Edit ";
				cancelButton.Visibility = Visibility.Hidden;
				ModifyUnderlyingTransaction(transactionId);
			}

		}

		bool IsCancelButton(UIElement e)
		{
			if (e is Button)
			{
				var b = e as Button;
				return b.Content.ToString().Trim() == "Cancel";
			}
			return false;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			ModifyUnderlyingTransaction((int)((Button)sender).Tag);
		}



		void ModifyUnderlyingTransaction(int transactionId, bool modify = false)
		{
			Transaction trn = Transactions.Find(t => t.TransactionId == transactionId);
			MainGrid.Children.FindAllLevelChildrenByType<Button>(b => b.Content.ToString().Trim() == "Save").ForEach(b => b.Content = "  Edit  ");
			string transactionTyp = string.Empty;
			MainGrid.Children.FindAllLevelChildrenByType<Button>(b => b.Content.ToString().Trim() == "Cancel").ForEach(b => b.Visibility = Visibility.Hidden);
			MainGrid.Children.FindAllLevelChildrenByType<SFComboBox>(c => { return c.TransactionId == transactionId; }).ForEach(cb => { cb.Visibility = Visibility.Hidden; if (modify) transactionTyp = cb.SelectedValue.ToString(); });
			MainGrid.Children.FindAllLevelChildrenByType<SFTextBox>(t => { return t.TransactionId == transactionId; }).ForEach(t =>
			{
				t.BorderThickness = new Thickness(0);
				t.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
				t.IsReadOnly = true;
				switch (t.Field)
				{
					case TransactionColumn.Date:
						if (modify)
						{
							DateTime dt = DateTime.Today;
							DateTime.TryParse(t.Text, out dt);
							trn.TransactionDate = dt;
						}
						else
						{
							t.Text = trn.TransactionDate.ToShortDateString();
						}
						break;

					case TransactionColumn.Transaction:
						if (modify)
						{
							TradeType trtyp = TradeType.Sell;
							Enum.TryParse<TradeType>(transactionTyp, out trtyp);
							trn.TransactionType = trtyp;
							t.Text = transactionTyp;
						}

						else
							t.Text = trn.TransactionType.ToString();
						break;
					default:
						break;
				}
			});

			MainGrid.Children.FindAllLevelChildrenByType<SFNTextBox>(t => { return t.TransactionId == transactionId; }).ForEach(t =>
			{
				t.BorderThickness = new Thickness(0);
				t.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
				t.IsReadOnly = true;
				switch (t.Field)
				{

					case TransactionColumn.Amount:
						if (modify)
						{
							trn.Amount = Convert.ToDecimal(t.Text);
						}
						else
							t.Text = trn.Amount.ToString("0.00");
						break;
					case TransactionColumn.Quantity:
						if (modify)
						{
							trn.Quantity = Convert.ToInt32(t.Text);

						}
						else
							t.Text = trn.Quantity.ToString("0");
						break;
					case TransactionColumn.TotalAmount:

						t.Text = (trn.Amount * trn.Quantity).ToString("0.00");
						break;
					default:
						break;
				}
			});

		}


		public event EventHandler CloseButtonClicked;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			CloseButtonClicked?.Invoke(sender, e);
		}
	}
}
