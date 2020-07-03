 

namespace StockFocus.Entites
{
    public class Portfolio
    {
      
        public int PortfolioId { get; set; }

    
        public string PortfolioName { get; set; }


        public bool IsDefault { get; set; }


		public override string ToString()
		{
            return PortfolioName;
		}

	}
}