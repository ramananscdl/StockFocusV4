using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public interface IDataLayer
    {



        int AddPortfolio(Portfolio portfolio);
     
        int AddStock(Stock stock);
   
        int AddTransaction(Transaction transaction);
      
        int AddSegment(Segment segment);


        bool EditPortfolio(Portfolio portfolio);

        bool EditStock(Stock stock);

        bool EditTransaction(Transaction transaction);

        bool EditSegment(Segment segment);



        List<Portfolio> GetPortfolios();
        List<Transaction> GetTransactionsByPortfolio(DateTime StartDate, DateTime EndDate, int PortfolioId);
        List<Stock> GetStocksByPortfolioId(int portfolioId);

        List<Transaction> GetAllTransactionsByStockId(int stockId, int portfolioId);

        List<Segment> GetAllSegments();

        List<Stock> GetStockList();

        decimal GetNetRealizedProfitByProfileId(int portfolioId);

    }
}
