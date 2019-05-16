using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockFocus.Entites;
using StockFocus.Helper;
using Newtonsoft.Json.Linq;

namespace StockFocus.Service
{
    public class AlphaVantageService : IStockService
    {
        const string baseUrl = @"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={0}.{1}&apikey=F7YMQ8K4LSQLW0SQ";

        public decimal GetNiftyPoints()
        {
            return 0;
        }

        public async Task<Stock> GetOnlineStockDetails(Stock stock)
        {
            try
            {
                string response = await Task.Run(() => WebHelper.GetWebResponse(string.Format(baseUrl, stock.Code.Code, stock.StockExchange == Exchange.NSE ? "NS" : "BSE")).Result);
                JObject res = JObject.Parse(response);
              
                stock.DayHigh = res.First.First.First.Next.Next.First.ToDecimal();
                stock.DayLow = res.First.First.First.Next.Next.Next.First.ToDecimal();
                stock.CMP = res.First.First.First.Next.Next.Next.Next.First.ToDecimal();
                stock.Close = res.First.First.First.Next.Next.Next.Next.Next.Next.Next.First.ToDecimal();
                stock.Change = stock.CMP - stock.Close;



            }
            catch (Exception ex)
            {
 
            }
            return stock;
        }

        public Task<decimal> GetStockPrice(string symbol, bool IsNSE)
        {
            throw new NotImplementedException();
        }
    }
}
