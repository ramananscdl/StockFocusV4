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

        public Stock Stock { get; set; }

        public int PortfolioId { get; set; }

        public IDataLayer DataLayer { get; set; }

        public void DataBind()
        {
            string[] cbxArray = { "Buy", "Sell" };
            MainGrid.Children.Clear(u => u is TextBox);
            this.Height = 50;
            this.txtStockName.Content = Stock.StockName;
            txtRate.Content = "Avg.Price:" + Stock.AdjustedPrice.ToString(" ₹ 0.00");
            txtQty.Content = "Qty:" + Stock.Quantity.ToString();

            var transactions = DataLayer.GetAllTransactionsByStockId(Stock.StockId, PortfolioId);
            int count = 1;
            foreach (var tr in transactions)
            {
                count++;
                RowDefinition rd = new RowDefinition() { Height = new GridLength(35) };
                MainGrid.RowDefinitions.Add(rd);
                TextBox tbDate = new TextBox() { Text = tr.TransactionDate.ToString("dd-MMM-yyyy"), HorizontalAlignment = HorizontalAlignment.Center, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), Tag = count };
                TextBox tbAmount = new TextBox() { Text = tr.Amount.ToString("₹ 0.00"), HorizontalAlignment = HorizontalAlignment.Right, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), Tag = count };
                TextBox tbQty = new TextBox() { Text = tr.Quantity.ToString(), HorizontalAlignment = HorizontalAlignment.Right, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), Tag = count };
                TextBox tbTotal = new TextBox() { Text = (tr.Quantity * tr.Amount).ToString("₹ 0.00"), HorizontalAlignment = HorizontalAlignment.Right, IsReadOnly = true, Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), BorderThickness = new Thickness(0), Tag = count };
                Label tbEmpty = new Label() { Content = "--", HorizontalAlignment = HorizontalAlignment.Center };
                Button btnEdit = new Button() { Content = "Edit", Height = 30, Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader"), Tag = tr.TransactionId };
                Button btnCancel = new Button() { Content = "Cancel", Height = 30, Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader"), Visibility = Visibility.Hidden, Tag = count };
                ComboBox cbx = new ComboBox() { Height = 30, ItemsSource = cbxArray , Visibility = Visibility.Hidden, Tag = count};
                cbx.SelectedValue = (tr.TransactionId == 1 ? "Buy" : "Sell");
               
                btnCancel.Click += BtnCancel_Click;

                btnEdit.Click += BtnEdit_Click;

                Brush cvBg = Background = Brushes.Wheat;

                Canvas cvDate = new Canvas() { Background = cvBg }; cvDate.Children.Add(tbDate);
                Canvas cvAmount = new Canvas() { Background = cvBg }; cvAmount.Children.Add(tbAmount);
                Canvas cvQty = new Canvas() { Background = cvBg }; cvQty.Children.Add(tbQty);
                Canvas cvTotal = new Canvas() { Background = cvBg }; cvTotal.Children.Add(tbTotal);
                Canvas cvEmpty = new Canvas() { Background = cvBg }; cvEmpty.Children.Add(tbEmpty);
                Canvas cvBtns = new Canvas() { Background = cvBg }; cvBtns.Children.Add(btnEdit); cvBtns.Children.Add(btnCancel); cvBtns.Children.Add(cbx);

                Canvas.SetLeft(btnEdit, 80);
                Canvas.SetLeft(btnCancel, 120);


                Grid.SetColumn(cvDate, 0);
                Grid.SetColumn(cvAmount, 3);
                Grid.SetColumn(cvTotal, 4);
                Grid.SetColumn(cvBtns, 5);

                Grid.SetColumn(cvEmpty, tr.TransactionType == TradeType.Sell ? 1 : 2);
                Grid.SetColumn(cvQty, tr.TransactionType == TradeType.Buy ? 1 : 2);

                Grid.SetRow(cvDate, count);
                Grid.SetRow(cvAmount, count);
                Grid.SetRow(cvTotal, count);
                Grid.SetRow(cvQty, count);
                Grid.SetRow(cvEmpty, count);
                Grid.SetRow(cvBtns, count);
                this.Height += 37;

                MainGrid.Children.Add(cvDate);
                MainGrid.Children.Add(cvAmount);
                MainGrid.Children.Add(cvTotal);
                MainGrid.Children.Add(cvQty);
                MainGrid.Children.Add(cvBtns);
                MainGrid.Children.Add(cvEmpty);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {


            var cancelButton = (((Button)sender).Parent as Canvas).Children.FindFirst<Button>(IsCancelButton);
            string process = ((Button)sender).Content.ToString();
            if (process == "Edit")
            {
                CancelClicked();
                ((Button)sender).Content = "Save";
                cancelButton.Visibility = Visibility.Visible;
                int row = (int)cancelButton.Tag;
                int transactionId = (int)((Button)sender).Tag;
                MainGrid.Children.FindAllLevelChildrenByType<TextBox>(t => (int)t.Tag == row).ForEach(t =>
                {
                    t.BorderThickness = new Thickness(1);
                    t.Background = Brushes.White;
                    t.IsReadOnly = false;
                });
                MainGrid.Children.FindAllLevelChildrenByType<ComboBox>(c => (int)((c as ComboBox).Tag) == row)[0].Visibility = Visibility.Visible;
            }
            else
            {
                ((Button)sender).Content = "Edit";
                cancelButton.Visibility = Visibility.Hidden;
                CancelClicked();
            }

        }

        bool IsCancelButton(UIElement e)
        {
            if (e is Button)
            {
                var b = e as Button;
                return b.Content.ToString() == "Cancel";
            }
            return false;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClicked();
        }

        void CancelClicked()
        {
            MainGrid.Children.FindAllLevelChildrenByType<Button>(b => b.Content.ToString() == "Save").ForEach(b => b.Content = "Edit");
            MainGrid.Children.FindAllLevelChildrenByType<Button>(b => b.Content.ToString() == "Cancel").ForEach(b => b.Visibility = Visibility.Hidden);
            MainGrid.Children.FindAllLevelChildrenByType<ComboBox>(c=> { return true; })[0].Visibility = Visibility.Hidden;
            MainGrid.Children.FindAllLevelChildrenByType<TextBox>(t => { return true; }).ForEach(t =>
            {
                t.BorderThickness = new Thickness(0);
                t.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                t.IsReadOnly = true;
            });
        }


        public event EventHandler CloseButtonClicked;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseButtonClicked?.Invoke(sender, e);
        }
    }
}
