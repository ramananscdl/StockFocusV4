using StockFocus.Entites;
using System.Threading.Tasks;

namespace StockFocus.Service
{
    public interface IStockService
    {
        Task<Stock> GetOnlineStockDetails(Stock stock);
        Task<decimal> GetStockPrice(string symbol, bool IsNSE);

        Task<Index> GetIndexPoints(bool isNifty = true);
    }
}