using System;
using System.Collections.Generic;

using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
	public class Stock
	{
		Symbol s;

		public Symbol Code
		{
			get
			{
				if (s == null) s = new Symbol();

				if (!string.IsNullOrEmpty(NSECode))
				{
					s.Code = NSECode;
					s.StockExchange = Exchange.NSE;
				}
				else
				{
					s.Code = BSECode;
					s.StockExchange = Exchange.BSE;


				}
				//  s.StockExchange = this.StockExchange;
				return s;
			}
		}

		public Exchange StockExchange { get; set; }


		public string StockName { get; set; }


		public decimal CMP { get; set; }

		public decimal Change { get; set; }

		public decimal DifferenceInPercentage
		{
			get
			{
				if (CMP > 0)
					return (Change / CMP * 100);
				else
					return 0;
			}
		}



		public int SegmentId { get; set; }

		public decimal Close { get; set; }

		public decimal PercChange200DMA { get; set; }

		public decimal MTMProfit { get; set; }

		public decimal YearHigh { get; set; }

		string nseCode = string.Empty;
		public string NSECode { get => nseCode; set => nseCode =value; }
		 
		string bseCode = string.Empty;
		public string BSECode { get => bseCode; set => bseCode = value; }


		public decimal YearLow { get; set; }
		public decimal TwoHundredDMA { get; set; }
		public decimal TransactionProfit { get; set; }
		public decimal PurchasePrice { get; set; }
		public decimal AdjustedPrice { get; set; }
		public decimal DayHigh { get; set; }

		public decimal DayLow { get; set; }

		public int Quantity { get; set; }

		public decimal CurrentValue
		{
			get
			{
				return Quantity * CMP;
			}
		}

		public decimal InvestedValue
		{
			get
			{
				return Quantity * AdjustedPrice;
			}
		}


		public decimal NetProfit
		{
			get
			{
				return CurrentValue - InvestedValue;
			}
		}

		public int StockId { get; set; }


		public override string ToString()
		{
			return this.Code.Code + " :" + this.CurrentValue.ToString();
		}

		public override bool Equals(object obj)
		{
			Stock s = obj as Stock;
			if (s is Stock)
				return this.StockId == s.StockId;

			return false;
		}

		public decimal ChangeFromBuyPrice
		{
			get
			{

				return CurrentValue == 0 || InvestedValue == 0 ? 0 : (CurrentValue - InvestedValue) / InvestedValue * 100;
			}
		}


		decimal _portfolioCurrentVal = 0;
		decimal _portfolioInvestedVal = 0;

		public void SetPortfolioValues(decimal portfolioCurrentVal, decimal portfolioInvestedVal)
		{
			_portfolioCurrentVal = portfolioCurrentVal;
			_portfolioInvestedVal = portfolioInvestedVal;
		}


		public decimal CurrentWeightage
		{
			get
			{
				if (_portfolioCurrentVal == 0) return 0;
				return CurrentValue / _portfolioCurrentVal * 100;
			}
		}


		public decimal InvestedWeightage
		{
			get
			{
				if (_portfolioInvestedVal == 0) return 0;
				return InvestedValue / _portfolioInvestedVal * 100;
			}
		}

	}
}
