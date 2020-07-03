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
using System.CodeDom;
using System.Windows.Navigation;
using System.Configuration;
using System.IO;

namespace StockFocusV4
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		System.Timers.Timer mainTimer = new System.Timers.Timer(600000);

		public MainWindow()
		{


			Thread.Sleep(1000);
			LoadSettings();
			if (BaseSettings.RefreshInterval < 100000)
			{
				mainTimer.Interval = BaseSettings.RefreshInterval * 60000;
				mainTimer.Elapsed += MainTimer_Elapsed;
				mainTimer.Start();
			}
			InitializeComponent();
			CurrentOrder = new GridTag("DifferenceInPercentage", GridSortOrder.Ascending);

			try
			{
				LoadPortfolio();
				btntest_Click(this, null);
				txtLastDate.Text = DataLayer.GetLastUpdatedDate().ToString("dd-MMM-yyyy");
			}
			catch (Exception ex)
			{
				StatusText.Text = ex.Message;
			}


		}

		private void LoadSettings()
		{
			string xml = string.Empty;
			using (FileStream fs = new FileStream("StockFocus.sfc", FileMode.Open, FileAccess.Read))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					xml = sr.ReadToEnd();

				}
			}
			BaseSettings = Serializer.DeserializeObject<SettingBase>(xml);

		}

		public SettingBase BaseSettings { get; set; }


		private void LoadPortfolio()
		{


			stpProfiles.Children.Clear();
			portFolios = DataLayer.GetPortfolios();
			portFolios.ForEach(p =>
			{
				string templ = p.IsDefault ? "MenuBtnHL" : "MenuBtn";
				Button btn = new Button()
				{
					Content = new TextBlock()
					{
						Style = (Style)Application.Current.FindResource(templ),
						Text = p.PortfolioName,
						Foreground = (Brush)Application.Current.FindResource("brCbxItemBg")
					},
					Width = stpProfiles.Width,
					Tag = p.PortfolioId,

				};
				btn.Click += ProfileChange_Clicked;

				stpProfiles.Children.Add(btn);

				if (p.IsDefault)
				{
					portfolioId = p.PortfolioId;
					txtPFName.Text = p.PortfolioName;
				}

			});


		}

		private void ProfileChange_Clicked(object sender, RoutedEventArgs e)
		{
			portfolioId = (int)((Button)sender).Tag;
			portFolios.ForEach(p => p.IsDefault = false);
			portFolios.Find(p => p.PortfolioId == portfolioId).IsDefault = true;
			txtPFName.Text = portFolios.Find(p => p.PortfolioId == portfolioId).PortfolioName;
			Dispatcher.BeginInvoke(new ThreadStart(() =>
			{
				StatusText.Text = "Fetching data...";
				var stocks = GetDetails().ContinueWith(Display);
			}));
		}

		private void MainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Dispatcher.BeginInvoke(new ThreadStart(() =>
			  {
				  StatusText.Text = "Fetching data...";
				  var stocks = GetDetails().ContinueWith(Display).ContinueWith((d) => { });
			  }));

		}


		public List<Portfolio> portFolios { get; set; }


		IDataLayer dataLayer = new SQLDataLayer();
		IStockService service = new YahooStockService();
		AlphaVantageService alphaService = new AlphaVantageService();
		private void btntest_Click(object sender, RoutedEventArgs e)
		{

			StatusText.Text = "Fetching data...";
			var stocks = GetDetails().ContinueWith(Display).ContinueWith((d) => { });


		}

		public Task<List<Stock>> GridStocks { get; set; }


		int portfolioId = 0;


		bool toggle = true;

		public IDataLayer DataLayer
		{
			get
			{
				dataLayer.ConnectionString = BaseSettings.DBConnectionString;
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


		Index NiftyIndex = null;
		Index SesexIndex = null;

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
				try
				{

					grdStocks.Children.Clear();
					grdStocks.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
					List<SfGridColumn> columns = BaseSettings.DisplayColumns;

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

					DisplayNetProfitLoss(netProfit, totInvestedVal <= 0 ? 0 : netProfit / totInvestedVal * 100);

					txtNiftyTitle.Style = (Style)Application.Current.FindResource(NiftyIndex.ChangePercentage > 0 ? "IndexTitleTextGreen" : "IndexTitleTextRed");
					txtNiftyVal.Text = NiftyIndex.CMP.ToString();
					txtNiftyChange.Text = string.Format("{0} ({1})", NiftyIndex.Change, NiftyIndex.ChangePercentage.ToString("P"));


					txtSensexTitle.Style = (Style)Application.Current.FindResource(SesexIndex.ChangePercentage > 0 ? "IndexTitleTextGreen" : "IndexTitleTextRed");
					txtSensexVal.Text = SesexIndex.CMP.ToString();
					txtSensexChange.Text = string.Format("{0} ({1})", SesexIndex.Change, SesexIndex.ChangePercentage.ToString("P"));

					prgProgress.Visibility = Visibility.Collapsed;


					foreach (var st in endResult)
					{
						if (st.Quantity > 0)
						{
							rowCount++;
							RowDefinition rd = new RowDefinition() { Height = new GridLength(25) };

							st.SetPortfolioValues(totalVal, totInvestedVal);

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
					StatusText.Text = string.Format("Retrived {0} stock(s) @ {1}", res.Result.Count, DateTime.Now.ToString("HH:mm"));
				}
				catch (Exception ex)
				{
					StatusText.Text = ex.Message;
				}
			}

		  ));

		}





		private void DisplayNetProfitLoss(decimal netProfit, decimal change)
		{
			if (netProfit > 0)
			{
				txtNPText.Text = "Net Profit";
				txtNetProfit.Foreground = Brushes.DarkGreen;
				txtNetProfit.Text = netProfit.ToString("₹ 0.00");
				txtNPPerc.Text = " (" + change.ToString("0.00") + "%)";
				txtNPPerc.Foreground = Brushes.DarkGreen;
			}
			else
			{
				txtNPText.Text = "Net Loss";
				txtNPPerc.Foreground = txtNetProfit.Foreground = Brushes.DarkMagenta;
				txtNetProfit.Text = netProfit.ToString("₹ 0.00");
				txtNPPerc.Text = " (" + change.ToString("0.00") + "%)";
			}
		}

		private void CreateHeaders(List<SfGridColumn> columns)
		{
			for (int i = 0; i < columns.Count; i++)
			{
				Button button = null;
				if (CurrentOrder.ColumnName == columns[i].ColumnName)
				{
					button = new Button() { Content = " " + columns[i].DisplayName, Tag = new GridTag(columns[i].ColumnName, CurrentOrder.Order), Template = (ControlTemplate)Application.Current.FindResource(CurrentOrder.Order == GridSortOrder.Ascending ? "btnGridHeaderUP" : "btnGridHeaderDOWN"), Foreground = Brushes.Black, Background = (Brush)Application.Current.FindResource("SFGridHeader") };

				}
				else
				{
					button = new Button() { Content = " " + columns[i].DisplayName, Tag = new GridTag(columns[i].ColumnName, GridSortOrder.None), Template = (ControlTemplate)Application.Current.FindResource("btnGridHeader"), Foreground = Brushes.Black, Background = (Brush)Application.Current.FindResource("SFGridHeader") };

				}

				button.Click += Grid_Header_Button_Clicked;
				grdStocks.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(columns[i].ColumnName == "StockName" ? 260 : 130) });
				Canvas c = new Canvas() { Background = (Brush)Application.Current.FindResource("SFGridHeader") };
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
				b.Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Menu } ;
				b.Click += TransactionButtonClicked;
				b.Tag = ((TextBlock)element).Tag;
				b.Background = Brushes.Wheat;
				b.Foreground = Brushes.Teal;
				b.VerticalAlignment = VerticalAlignment.Center;
				b.Height = 24;
				b.Template = (ControlTemplate)Application.Current.FindResource("btnTopMenus");
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
			Stock stk = (Stock)((Button)sender).Tag;
			TransDetailsWindow.Stock = stk;

			TransDetailsWindow.Transactions = DataLayer.GetAllTransactionsByStockId(stk.StockId, PortfolioId);

			TransDetailsWindow.PortfolioId = PortfolioId;
			TransDetailsWindow.DataBind();
			AnimationHelper.SlideAnimate(TransWindow, 0.5, -1700, 0, new   CubicEase() { EasingMode = EasingMode.EaseInOut }, false);
		}

		private void TransDetailsWindow_CloseButtonClicked(object sender, EventArgs e)
		{
			AnimationHelper.SlideAnimate(TransWindow, 1, 1700, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
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

		// YahooStockService yService = new YahooStockService();

		public async Task<List<Stock>> GetDetails()
		{
			prgProgress.Visibility = Visibility.Visible;
			List<Stock> stocks = DataLayer.GetStocksByPortfolioId(PortfolioId);
			await Task<decimal>.Factory.StartNew(() => NRP = DataLayer.GetNetRealizedProfitByProfileId(portfolioId));

			decimal sceenableStock = stocks.Count(s => s.Quantity > 0) + 2;
			StatusText.Text = "Started browsing";
			decimal fetchedCount = 0;
			foreach (var stk in stocks)
			{
				if (stk.Quantity != 0)
				{
					try
					{
						//  var t = await Task<Stock>.Factory.StartNew(() => alphaService.GetOnlineStockDetails(stk).Result);

						//yService.GetOnlineStockDetailsDirect(stk);
						if (stk.CMP <= 0)
						{
							// Service.GetOnlineStockDe(stk);
							await Task<Stock>.Factory.StartNew(() => Service.GetOnlineStockDetails(stk).Result);
						}
						fetchedCount++;
						StatusText.Text = string.Format("Fetched '{0}' details", stk.StockName);
						prgProgress.Value = (double)(fetchedCount / sceenableStock * 100);
					}
					catch (Exception ex)
					{
						StatusText.Text = " Error occured:" + ex.Message;
					}
				}
			}

			NiftyIndex = await Service.GetIndexPoints();
			StatusText.Text = string.Format("Fetching Nifty Values");
			prgProgress.Value = (double)((fetchedCount + 1) / sceenableStock * 100);
			SesexIndex = await service.GetIndexPoints(false);
			StatusText.Text = string.Format("Fetching Sensex values");
			prgProgress.Value = (double)((fetchedCount + 2) / sceenableStock * 100);


			return stocks;


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
					tr.PortfolioId = GetInputValueByName<int>(args.Data, "Portfolio"); ;
					var TrId = DataLayer.AddTransaction(tr);
					MessageBox.Show(string.Format("Added Stock transaction  as   ID :{0}", TrId), "Added Transaction", MessageBoxButton.OK);
					txtLastDate.Text = DateTime.Today.ToString("dd-MMM-yyyy");
					break;
				case InputOperation.AddProfile:
					Portfolio pf = new Portfolio();
					pf.PortfolioName = GetInputValueByName<string>(args.Data, "PortfolioName");
					pf.IsDefault = GetInputValueByName<int>(args.Data, "IsDefault") == 1;
					var pfId = DataLayer.AddPortfolio(pf);
					MessageBox.Show(string.Format("Added Portfolio {1} as {0}", pf.PortfolioName, pfId));
					break;
				case InputOperation.AddSegment:
					Segment seg = new Segment();
					seg.SegmentName = GetInputValueByName<string>(args.Data, "SegmentName");
					var segId = dataLayer.AddSegment(seg);
					MessageBox.Show(string.Format("Added Segment {0} with ID :{1}", seg.SegmentName, segId), "Added Segment", MessageBoxButton.OK);
					LoadPortfolio();
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



		private void btnAddTransaction_Click(object sender, RoutedEventArgs e)
		{
			List<GridItem> items = new List<GridItem>() {
				new GridItem() {DisplayName ="Stock", ItemName="Stock", InputControl = InputType.DropDown, ControlValues=GeneralHelper.GetKVPCollection(DataLayer.GetStockList()) },
				new GridItem() { DisplayName = "Transaction Date", ItemName="TD", InputControl = InputType.Calendar },
				new GridItem() { DisplayName="Quantity", ItemName="Qty", InputControl = InputType.NumericTextbox, DecimalNumbers = 0 },
				new GridItem() {DisplayName="Trade Type",ItemName="TT", InputControl= InputType.DropDown, ControlValues = GeneralHelper.GetKVPCollectionFromEnum(typeof(TradeType)) },
				new GridItem() {DisplayName ="Price", ItemName="Price", InputControl= InputType.NumericTextbox, DecimalNumbers = 2 },
				new GridItem() {DisplayName="Portfolio", ItemName="Portfolio", InputControl = InputType.DropDown,ControlValues=  GeneralHelper.GetKVPCollection(DataLayer.GetPortfolios() )   }
			};
			UcInsertForm.DataItems = items;
			UcInsertForm.Title = "Add Transaction";
			UcInsertForm.Operation = InputOperation.AddTransaction;
			UcInsertForm.DataBind();
			AnimationHelper.SlideAnimate(UcInsertForm, 0.25, -1300, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
		}


		private void Portfolio_Changed(object sender, RoutedEventArgs e)
		{

		}


		private void btnManagePortfolio_Click(object sender, RoutedEventArgs e)
		{
			List<Portfolio> portfolios = DataLayer.GetPortfolios();

			UcManageForm.Title = "Portfolio";
			UcManageForm.Description = "Edit/Delete";
			UcManageForm.KeyColumn = "PortfolioId";
			UcManageForm.ValueColumn = "PortfolioName";
			UcManageForm.DataBind<Portfolio>(portfolios);
			UcManageForm.DisplayColumns = new string[] { "PortfolioId", "PortfolioName", "IsDefault" };
			UcManageForm.ColumnMaxSize = new List<KeyValuePair<string, int>>() { new KeyValuePair<string, int>("PortfolioName", 12) };
			AnimationHelper.SlideAnimate(UcManageForm, 0.25, -1800, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
		}

		private void UcManageForm_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void UcManageForm_CancelClicked(object sender, EventArgs e)
		{
			AnimationHelper.SlideAnimate(UcManageForm, 1, 1800, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
		}

		private void UcManageForm_SaveClicked(object sender, ManageEventArgs Data)
		{
			if (Data.Operation == ManageOperation.Edit)
			{
				if (Data.UnderlyingObject == typeof(Portfolio))
				{
					if (DataLayer.EditPortfolio((Portfolio)Data.Data))
						MessageBox.Show("Successfully Updated Portfolio");
					LoadPortfolio();
				}
				else if (Data.UnderlyingObject == typeof(Stock))
				{
					if (DataLayer.EditStock((Stock)Data.Data))
						MessageBox.Show("Successfully Updated Stock");
					LoadPortfolio();
				}
			}
			else if (Data.Operation == ManageOperation.Delete)
			{
				if (Data.UnderlyingObject == typeof(Portfolio))
				{
					if (DataLayer.DeletePortfolio(((Portfolio)Data.Data).PortfolioId))
						MessageBox.Show("Successfully Deleted Portfolio");
					//LoadPortfolio();
				}
			}

		}

		private void UcManageForm_DeleteClicked(object sender, ManageEventArgs Data)
		{
			var data = Data.Data;
		}

		private void TransDetailsWindow_SaveClicked(object sender, ManageEventArgs<Transaction> Data)
		{

			if (Data.Operation == ManageOperation.Edit)
			{
				if (!DataLayer.EditTransaction(Data.ObjValue))
					MessageBox.Show("Operation Failed");
			}
		}

		private void TransDetailsWindow_DeleteClicked(object sender, ManageEventArgs<Transaction> Data)
		{
			if (!DataLayer.DeleteTransaction(Data.ObjValue.TransactionId))
				MessageBox.Show("Delete Failed");
		}



		private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
		{
			ButtonCloseMenu.Visibility = Visibility.Visible;
			ButtonOpenMenu.Visibility = Visibility.Collapsed;
		}

		private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
		{
			ButtonCloseMenu.Visibility = Visibility.Collapsed;
			ButtonOpenMenu.Visibility = Visibility.Visible;
		}

		private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{



			switch (((ListViewItem)((ListView)sender).SelectedItem).Tag.ToString())
			{
				case "AddStock":
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
					break;
				case "ManageStock":
					List<Stock> stocks = DataLayer.GetStockList();
					UcManageForm.Description = "Edit/Delete ";
					UcManageForm.KeyColumn = "StockId";
					UcManageForm.ValueColumn = "StockName";
					UcManageForm.DataBind<Stock>(stocks);
					UcManageForm.DisplayItems = new List<DisplayItem>() {
					new DisplayItem() { PropertyName = "StockExchange", AlternateName = "Exchange", DataSource = GeneralHelper.GetKVPCollectionFromEnum(typeof(Exchange)) }
					, new DisplayItem() {PropertyName ="SegmentId", AlternateName ="Segment", DataSource = GeneralHelper.GetKVPCollection(DataLayer.GetAllSegments()) } };
					UcManageForm.DisplayColumns = new string[] { "StockId", "StockName", "NSECode", "BSECode", "StockExchange", "SegmentId" };
					AnimationHelper.SlideAnimate(UcManageForm, 0.25, -1800, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
					break;
				case "AddSegment":
					List<GridItem> segmentItems = new List<GridItem>() {
				new GridItem() { DisplayName = "Segment Name", ItemName="SegmentName", InputControl = InputType.Textbox }           };
					UcInsertForm.DataItems = segmentItems;
					UcInsertForm.Title = "Add Segment";
					UcInsertForm.Operation = InputOperation.AddSegment;
					UcInsertForm.DataBind();
					AnimationHelper.SlideAnimate(UcInsertForm, 0.25, -1300, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
					break;
				case "ManageSegment":
					List<Segment> segments = DataLayer.GetAllSegments();

					UcManageForm.Title = "Segment";
					UcManageForm.Description = "Edit/Delete ";
					UcManageForm.KeyColumn = "SegmentId";
					UcManageForm.ValueColumn = "SegmentName";
					UcManageForm.DataBind<Segment>(segments);
					UcManageForm.DisplayColumns = new string[] { "SegmentName", "SegmentId" };
					AnimationHelper.SlideAnimate(UcManageForm, 0.25, -1800, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
					break;

				case "AddPortfolio":
					List<GridItem> portfolioItems = new List<GridItem>() {
				new GridItem() { DisplayName = "Portfolio Name[12]", ItemName="PortfolioName", InputControl = InputType.Textbox } ,
					new GridItem(){DisplayName="Make this as default profile" , ItemName="IsDefault",
						InputControl = InputType.DropDown, ControlValues = new List<KeyValuePair<int, string>> ()
						{ new KeyValuePair<int, string>(1, "Yes"), new KeyValuePair<int, string>(0,"No") } } };

					UcInsertForm.DataItems = portfolioItems;
					UcInsertForm.Title = "Add Portfolio";
					UcInsertForm.Operation = InputOperation.AddProfile;
					UcInsertForm.DataBind();
					AnimationHelper.SlideAnimate(UcInsertForm, 0.25, -1300, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
					break;
				default:
					break;
			}
		}



		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			ucSettings.BaseSettings = BaseSettings;
			AnimationHelper.SlideAnimate(ucSettings, 0.25, -1800, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
		}

		private void ucSettings_SaveClicked(object sender, SettingsEventArgs data)
		{
			BaseSettings = data.Settings;
			AnimationHelper.SlideAnimate(ucSettings, 1, 2400, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
			var xml = Serializer.SerializeObject<StockFocus.Entites.SettingBase>(BaseSettings);

			using (FileStream fs = new FileStream("StockFocus.sfc", FileMode.OpenOrCreate, FileAccess.Write))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write(xml);
				}
			}
		}

		private void ucSettings_CancelClicked(object sender, EventArgs e)
		{
			AnimationHelper.SlideAnimate(ucSettings, 1, 2400, 0, new QuarticEase() { EasingMode = EasingMode.EaseIn }, false);
		}
	}
}
