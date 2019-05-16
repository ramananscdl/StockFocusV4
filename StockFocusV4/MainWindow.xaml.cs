using StockFocus.Entites;
using StockFocus.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;
using StockFocus;
using System.Windows.Media.Animation;
using StockFocus.UI;
using System;
using StockFocus.Helper;

namespace StockFocusV4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer mainTimer = new System.Timers.Timer(120000);

        public MainWindow()
        {
            InitializeComponent();
            CurrentOrder = new GridTag("DifferenceInPercentage", GridSortOrder.Ascending);
            mainTimer.Elapsed += MainTimer_Elapsed;
            mainTimer.Start();
        }

        private void MainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
              {
                  StatusText.Text = "Fetching data...";
                  var stocks = GetDetails().ContinueWith(Display);
              }));
        }





        IDataLayer dataLayer = new SQLDataLayer();
        IStockService service = new NSEStockService();
        IStockService bseService = new AlphaVantageService();
        private void btntest_Click(object sender, RoutedEventArgs e)
        {

            StatusText.Text = "Fetching data...";
            var stocks = GetDetails().ContinueWith(Display);


        }

        public Task<List<Stock>> GridStocks { get; set; }


        int portfolioId = 1;


        bool toggle = true;

        public IDataLayer DataLayer
        {
            get
            {
                return dataLayer;
            }

            set
            {
                dataLayer = value;
            }
        }

        public IStockService Service
        {
            get
            {
                return service;
            }

            set
            {
                service = value;
            }
        }


        public GridTag CurrentOrder { get; set; }

        public int PortfolioId
        {
            get
            {
                return portfolioId;
            }

            set
            {
                portfolioId = value;
            }
        }

        public decimal NRP
        {
            get
            {
                return nRP;
            }

            set
            {
                nRP = value;
            }
        }

        public void Display(Task<List<Stock>> res)
        {
            GridStocks = res;
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {

                grdStocks.Children.Clear();
                grdStocks.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                List<SfGridColumn> columns = new List<SfGridColumn>() { new SfGridColumn("StockName", "Stock Name"), new SfGridColumn("CMP", "Last Price")
                    , new SfGridColumn("DifferenceInPercentage","Change %"), new SfGridColumn("Change" ), new SfGridColumn("Quantity") , new SfGridColumn("Close","Previous Close")
                    ,new SfGridColumn("DayLow","Day Low") , new SfGridColumn("DayHigh","Day High"), new  SfGridColumn("CurrentValue","Current Value"), new SfGridColumn("InvestedValue","Invested Value")
                   ,new SfGridColumn("NetProfit","Profit/Loss") , new SfGridColumn("AdjustedPrice", "Break Even")
                ,new SfGridColumn("PurchasePrice") };
                CreateHeaders(columns);
                int rowCount = 0;

                List<Stock> endResult = null;
                if (CurrentOrder.Order == GridSortOrder.Descending)
                    endResult = res.Result.OrderByDescending(s => s.GetType().GetProperty(CurrentOrder.ColumnName).GetValue(s)).ToList();
                else
                    endResult = res.Result.OrderBy(s => s.GetType().GetProperty(CurrentOrder.ColumnName).GetValue(s)).ToList();

                var totalVal = res.Result.Sum(s => s.CurrentValue);
                var totInvestedVal = res.Result.Sum(s => s.InvestedValue);

                txtTotalInvestedValue.Text = totInvestedVal.ToString(" ₹ 0.00");
                txtTotalCurrentValue.Text = totalVal.ToString(" ₹ 0.00");
                decimal netProfit = totalVal - totInvestedVal + NRP;
                DisplayNetProfitLoss(netProfit, netProfit / totInvestedVal*100);

                foreach (var st in endResult)
                {
                    if (st.Quantity > 0)
                    {
                        rowCount++;
                        RowDefinition rd = new RowDefinition() { Height = new GridLength(30) };



                        grdStocks.RowDefinitions.Add(rd);

                        for (int colCount = 0; colCount < columns.Count; colCount++)
                        {
                            PropertyInfo pi = st.GetType().GetProperty(columns[colCount].ColumnName);
                            TextBlock tb;
                            if (pi.PropertyType.Name == "Decimal")
                            {
                                tb = new TextBlock() { Text = ((decimal)pi.GetValue(st)).ToString("0.00"), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Style = (Style)Application.Current.FindResource("GridText") };
                            }
                            else
                            {
                                tb = new TextBlock() { Text = pi.GetValue(st).ToString(), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Style = (Style)Application.Current.FindResource("GridText") };
                                if (colCount == 0) tb.Tag = st;
                            }
                            SetRowColumnIndex(colCount, rowCount, tb, toggle ? "GridRowBG" : "GridRowAlternateBG", st.DifferenceInPercentage);
                        }
                        toggle = !toggle;
                    }
                }
                StatusText.Text = string.Format("Retrived {0} stock(s)", res.Result.Count);
            }));

        }





        private void DisplayNetProfitLoss(decimal netProfit, decimal change)
        {
            if (netProfit > 0)
            {
                txtNPText.Text = "Net Profit";
                txtNetProfit.Foreground = Brushes.LightCyan;
                txtNetProfit.Text = netProfit.ToString("Rs 0.00") + " (" + change.ToString("0.00") + "%)";
            }
            else
            {
                txtNPText.Text = "Net Loss";
                txtNetProfit.Foreground = Brushes.DarkMagenta;
                txtNetProfit.Text = netProfit.ToString("Rs 0.00") + Environment.NewLine+ " (" + change.ToString("0.00") + "%)"; ;
            }
        }

        private void CreateHeaders(List<SfGridColumn> columns)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                Button button = null;
                if (CurrentOrder.ColumnName == columns[i].ColumnName)
                {
                    button = new Button() { Content = " " + columns[i].DisplayName, Tag = new GridTag(columns[i].ColumnName, CurrentOrder.Order), Template = (ControlTemplate)Application.Current.FindResource(CurrentOrder.Order == GridSortOrder.Ascending ? "btnGridHeaderUP" : "btnGridHeaderDOWN"), Foreground = Brushes.Black, Background = (Brush)Application.Current.FindResource("GridHeader") };

                }
                else
                {
                    button = new Button() { Content = " " + columns[i].DisplayName, Tag = new GridTag(columns[i].ColumnName, GridSortOrder.None), Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader"), Foreground = Brushes.Black, Background = (Brush)Application.Current.FindResource("GridHeader") };

                }

                button.Click += Grid_Header_Button_Clicked;
                grdStocks.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(columns[i].ColumnName == "StockName" ? 260 : 130) });
                Canvas c = new Canvas() { Background = (Brush)Application.Current.FindResource("GridHeader") };
                Grid.SetColumn(button, i);
                Grid.SetRow(button, 0);
                Grid.SetColumn(c, i);
                Grid.SetRow(c, 0);
                grdStocks.Children.Add(c);
                grdStocks.Children.Add(button);

            }
        }

        private void Grid_Header_Button_Clicked(object sender, RoutedEventArgs e)
        {
            string columnName = ((GridTag)((Button)sender).Tag).ColumnName;
            if (columnName == CurrentOrder.ColumnName && CurrentOrder.Order == GridSortOrder.Ascending)
            {
                CurrentOrder.Order = GridSortOrder.Descending;
            }
            else
            {
                CurrentOrder.Order = GridSortOrder.Ascending;
                CurrentOrder.ColumnName = columnName;

            }
            Display(GridStocks);
        }

        private void SetRowColumnIndex(int column, int count, UIElement element, string rowcolor, decimal change)
        {

            Border border = new Border() { BorderThickness = new Thickness(0.2), BorderBrush = Brushes.White };
            Canvas c = new Canvas();
            c.Background = GetBackgroundColor(column, rowcolor, change);

            if (element is TextBlock && ((TextBlock)element).Tag != null)
            {
                Button b = new Button();
                b.Content = "...";
                b.Click += TransactionButtonClicked;
                b.Tag = ((TextBlock)element).Tag;
                b.Background = Brushes.Wheat;
                b.VerticalAlignment = VerticalAlignment.Center;
                // b.Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader");

                Grid g = new Grid();
                ColumnDefinition cdText = new ColumnDefinition() { Width = new GridLength(240) };
                ColumnDefinition cdbtn = new ColumnDefinition() { Width = new GridLength(20) };
                g.ColumnDefinitions.Add(cdText);
                g.ColumnDefinitions.Add(cdbtn);

                Grid.SetColumn(element, 0);
                Grid.SetColumn(b, 1);

                g.Children.Add(b);
                g.Children.Add(element);
                c.Children.Add(g);
            }
            else
            {

                Canvas.SetLeft(element, 10);
                Canvas.SetTop(element, 6);
                c.Children.Add(element);
            }


            Grid.SetColumn(c, column); Grid.SetRow(c, count);
            Grid.SetColumn(border, column); Grid.SetRow(border, count);

            grdStocks.Children.Add(c);
            grdStocks.Children.Add(border);
        }

        private void TransactionButtonClicked(object sender, RoutedEventArgs e)
        {

            TransDetailsWindow.Stock = (Stock)((Button)sender).Tag;
            TransDetailsWindow.DataLayer = this.DataLayer;
            TransDetailsWindow.PortfolioId = PortfolioId;
            TransDetailsWindow.DataBind();
            AnimationHelper.OpacityAnimate(TransWindow, 2, 100, new QuadraticEase());
        }

        private static Brush GetBackgroundColor(int column, string rowcolor, decimal change)
        {
            Brush canvasBrush;
            byte red, green;
            if (column == 0)
            {
                if (change > 10) change = 10; else if (change < -10) change = -10;

                if (change == 0) canvasBrush = Brushes.White;
                else if (change > 0)
                {

                    green = (byte)(205 + (change * 5));
                    byte otherVal = (byte)(200 - (10 * change));
                    canvasBrush = new SolidColorBrush(Color.FromRgb(otherVal, green, otherVal));
                }
                else
                {
                    red = (byte)(205 + (change * 5));
                    byte otherVal = (byte)(200 + (10 * change));
                    canvasBrush = new SolidColorBrush(Color.FromRgb(red, otherVal, otherVal));
                }

            }
            else
            {
                canvasBrush = (Brush)Application.Current.FindResource(rowcolor);
            }

            return canvasBrush;
        }

        decimal nRP;

        public async Task<List<Stock>> GetDetails()
        {
            List<Stock> stocks = DataLayer.GetStocksByPortfolioId(PortfolioId);
            await Task<decimal>.Factory.StartNew(()=> NRP = DataLayer.GetNetRealizedProfitByProfileId(portfolioId));
            foreach (var stk in stocks)
            {

                var t = await Task<Stock>.Factory.StartNew(() => Service.GetOnlineStockDetails(stk).Result);
                if (stk.CMP <= 0)
                {
                    await Task<Stock>.Factory.StartNew(() => bseService.GetOnlineStockDetails(stk).Result);
                }
            }
            return stocks;
        }



        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            List<GridItem> items = new List<GridItem>() {
                new GridItem() { DisplayName = "Stock Name", ItemName="StockName", InputControl = InputType.Textbox },
                    new GridItem() { DisplayName = "Code", ItemName = "Code", InputControl = InputType.Textbox },
                new GridItem() { DisplayName = "Segment", ItemName = "SegmentId", InputControl = InputType.DropDown,  ControlValues = GeneralHelper.GetKVPCollection(DataLayer.GetAllSegments()) },
                new GridItem() { DisplayName="Exchange", ItemName = "Exchange", InputControl = InputType.DropDown, ControlValues=  GeneralHelper.GetKVPCollectionFromEnum(typeof(Exchange)) } 

            };


            UcInsertForm.DataItems = items;
            UcInsertForm.Title = "Add Stock";
            UcInsertForm.Operation = InputOperation.AddStock;
            UcInsertForm.DataBind();
            AnimationHelper.SlideAnimate(UcInsertForm, 0.25, -1300, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);

        }

        private void UcInsertForm_CancelClicked(object sender, System.EventArgs e)
        {
            AnimationHelper.SlideAnimate(UcInsertForm, 1, 2400, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
        }

        private void UcInsertForm_SaveClicked(object sender, InputSavedEventArgs args)
        {

            switch (args.Operation)
            {
                case InputOperation.AddStock:
                    Stock stock = new Stock();
                    stock.StockName = GetInputValueByName<string>(args.Data, "StockName");



                    stock.SegmentId = GetInputValueByName<int>(args.Data, "SegmentId");
                    if (stock.Code.StockExchange == Exchange.NSE)
                        stock.NSECode = GetInputValueByName<string>(args.Data, "Code");
                    else
                        stock.BSECode = GetInputValueByName<string>(args.Data, "Code");
                    stock.StockExchange = (Exchange)Enum.Parse(typeof(Exchange), GetInputValueByName<string>(args.Data, "Exchange"));

                    if (ValidateScriptCode(stock.Code.Code))
                    {
                        var stkId = DataLayer.AddStock(stock);
                        MessageBox.Show(string.Format("Added Stock as {0}. ID :{1}", stock.StockName, stkId), "Added Stock", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Script  {0} is invalid. Please recheck the code or retry after sometime.", stock.StockName), "Script Code Error", MessageBoxButton.OK);
                    }

                    break;
                case InputOperation.AddTransaction:
                    Transaction tr = new Transaction();
                    tr.TransactionDate = GetInputValueByName<DateTime>(args.Data, "TD");
                    tr.Quantity = GetInputValueByName<int>(args.Data, "Qty");
                    tr.TransactionType = GetInputValueByName<TradeType>(args.Data, "TT");
                    tr.StockId = GetInputValueByName<int>(args.Data, "Stock");
                    tr.Amount = GetInputValueByName<decimal>(args.Data, "Price");
                    tr.PortfolioId = 1;
                    var TrId = DataLayer.AddTransaction(tr);
                    MessageBox.Show(string.Format("Added Stock transaction  as   ID :{0}", TrId), "Added Transaction", MessageBoxButton.OK);

                    break;
                case InputOperation.AddProfile:
                    break;
                case InputOperation.AddSegment:
                    Segment seg = new Segment();
                    seg.SegmentName = GetInputValueByName<string>(args.Data, "SegmentName");
                    var segId = dataLayer.AddSegment(seg);
                    MessageBox.Show(string.Format("Added Segment {0} with ID :{1}", seg.SegmentName, segId), "Added Segment", MessageBoxButton.OK);
                    break;
            }
            if (args.Operation != InputOperation.AddTransaction)
                AnimationHelper.SlideAnimate(UcInsertForm, 1, 2400, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
        }

        private bool ValidateScriptCode(string code)
        {
            // return Service.GetStockPrice(code).Result > 0;
            return true;
        }




        T GetInputValueByName<T>(List<GridItem> items, string name)
        {
            if (!typeof(T).IsEnum)
                return Convert.ChangeType(items.First(i => i.ItemName == name).ReturnValue, typeof(T));
            else
                return (T)(items.First(i => i.ItemName == name).ReturnValue);
        }

        private void btnAddSegment_Click(object sender, RoutedEventArgs e)
        {
            List<GridItem> items = new List<GridItem>() {
                new GridItem() { DisplayName = "Segment Name", ItemName="SegmentName", InputControl = InputType.Textbox }
            };
            UcInsertForm.DataItems = items;
            UcInsertForm.Title = "Add Segment";
            UcInsertForm.Operation = InputOperation.AddSegment;
            UcInsertForm.DataBind();
            AnimationHelper.SlideAnimate(UcInsertForm, 0.25, -1300, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
        }

        private void btnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            List<GridItem> items = new List<GridItem>() {
                new GridItem() {DisplayName ="Stock", ItemName="Stock", InputControl = InputType.DropDown, ControlValues=GeneralHelper.GetKVPCollection(DataLayer.GetStockList()) },
                new GridItem() { DisplayName = "Transaction Date", ItemName="TD", InputControl = InputType.Calendar },
                new GridItem() { DisplayName="Quantity", ItemName="Qty", InputControl = InputType.NumericTextbox, DecimalNumbers = 0 },
                new GridItem() {DisplayName="Trade Type",ItemName="TT", InputControl= InputType.DropDown, ControlValues = GeneralHelper.GetKVPCollectionFromEnum(typeof(TradeType)) },
                new GridItem() {DisplayName ="Price", ItemName="Price", InputControl= InputType.NumericTextbox, DecimalNumbers = 2 },
                new GridItem() {DisplayName="Portfolio", ItemName="Portfolio", InputControl = InputType.Textbox }
            };
            UcInsertForm.DataItems = items;
            UcInsertForm.Title = "Add Transaction";
            UcInsertForm.Operation = InputOperation.AddTransaction;
            UcInsertForm.DataBind();
            AnimationHelper.SlideAnimate(UcInsertForm, 0.25, -1300, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
        }

        private void TransDetailsWindow_CloseButtonClicked(object sender, EventArgs e)
        {
            AnimationHelper.OpacityAnimate(TransWindow, 0.25, 0, new ElasticEase() { EasingMode = EasingMode.EaseIn, Oscillations = 2, Springiness = 2.2 });
        }
    }
}
