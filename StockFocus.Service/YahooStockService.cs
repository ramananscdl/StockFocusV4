using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockFocus.Entites;
using StockFocus.Helper;

namespace StockFocus.Service
{
    public class YahooStockService : IStockService
    {

        const string baseUrl = @"https://finance.yahoo.com/quote/{0}{1}";



        public async Task<Stock> GetOnlineStockDetails(Stock stock)
        {

            try
            {
                string response = await Task.Run(() => WebHelper.GetWebResponse(string.Format(baseUrl, stock.Code.Code, stock.StockExchange == Exchange.NSE ? ".NS" : ".BO")).Result);
                response = response.Substring(response.IndexOf("Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)"));
                stock.CMP = GetDataByReactId("35", response).ToDecimal();
              //  stock.Close = GetDataByReactId("40", response).ToDecimal();
               // stock.Change = (stock.CMP - stock.Close) / stock.Close * 100;

            }
            catch (Exception ex)
            {
                stock.StockName = stock.StockName + " Not available or Error";
                ///TODO: Handle this error 
            }
            return stock;
        }

        private static string GetDataByReactId(string id, string response)
        {
            string badge = "data-reactid=\"" + id + "\">";

            string res = response.Substring(response.IndexOf(badge) + 18, 35);
            res = res.Substring(0, res.IndexOf("<"));
            return res;



        }

        public async Task<decimal> GetStockPrice(string symbol, bool IsNSE)
        {
            try
            {


                string response = await Task.Run(() => WebHelper.GetWebResponse(string.Format(baseUrl, IsNSE ? ".NS" : ".BO")).Result);
                response = response.Substring(response.IndexOf("Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)"));
                return GetDataByReactId("35", response).ToDecimal();

            }
            catch (Exception)
            {
                return "0.00".ToDecimal();
            }


        }

        public decimal GetNiftyPoints()
        {
            throw new NotImplementedException();
        }
    }
}
