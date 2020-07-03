using System;


namespace StockFocus.Entites
{
	public class Transaction
	{
		public DateTime TransactionDate { get; set; }

		public int Quantity { get; set; }

		public int StockId { get; set; }

		public TradeType TransactionType { get; set; }

		public decimal Amount { get; set; }

		public int PortfolioId { get; set; }

		public string Code { get; set; }
		 
		public int TransactionId { get; set; }

	 
		public string TransName
		{
			get
			{
				return string.Format("{0} on {1}- {2} count", Code, TransactionDate.ToString("dd-MMM-yyyy"),Quantity);
			}
		}

	}



}