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
                // string response = await Task.Run(() => WebHelper.GetWebResponseAsync(string.Format(baseUrl, stock.Code.Code.Trim(), stock.StockExchange == Exchange.NSE ? ".NS" : ".BO")));
                string response = WebHelper.GetWebResponse(string.Format(baseUrl, stock.Code.Code.Trim(), stock.StockExchange == Exchange.NSE ? ".NS" : ".BO"));
                response = response.Substring(response.IndexOf("Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)"));
                stock.CMP = GetDataByReactId("32", response).ToDecimal();
                string chg = GetDataByReactId("33", response);
                stock.Change = GetChange(chg);
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

        public Stock GetOnlineStockDetailsDirect(Stock stock)
        {

            try
            {
                string response = WebHelper.GetWebResponse(string.Format(baseUrl, stock.Code.Code.Trim(), stock.StockExchange == Exchange.NSE ? ".NS" : ".BO"));
                response = response.Substring(response.IndexOf("Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)"));
                stock.CMP = GetDataByReactId("34", response).ToDecimal();
                string chg = GetDataByReactId("35", response);
                stock.Change = GetChange(chg);
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

        private static decimal GetChange(string chg)
        {
            decimal d = 0;
            int startIndex = chg.IndexOf("(");
            string chgPer = chg.Substring(0, startIndex).Replace("+", "").Replace("%", "");
            decimal.TryParse(chgPer, out d);
            return d;


        }



        public async Task<decimal> GetStockPrice(string symbol, bool IsNSE)
        {
            try
            {


                string response = await Task.Run(() => WebHelper.GetWebResponse(string.Format(baseUrl, IsNSE ? ".NS" : ".BO")));
                response = response.Substring(response.IndexOf("Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)"));
                return GetDataByReactId("34", response).ToDecimal();

            }
            catch (Exception)
            {
                return "0.00".ToDecimal();
            }


        }

        public async Task<Index> GetIndexPoints(bool isNifty = true)
        {
            Index i = new Index();
            string indexUrl = isNifty ? @"https://finance.yahoo.com/quote/%5ENSEI/history?p=^NSEI&.tsrc=fin-srch" : @"https://finance.yahoo.com/quote/%5EBSESN/history?p=^BSESN&.tsrc=fin-srch";
            string response = await Task.Run(() => WebHelper.GetWebResponseAsync(indexUrl));
            response = response.Substring(response.IndexOf("Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)"));
            i.CMP = GetDataByReactId("32", response).ToDecimal();
            string chg = GetDataByReactId("33", response);
            i.Change = GetChange(chg);
            i.ChangePercentage = i.Change / i.CMP;
            i.IndexName = isNifty ? "NIFTY" : "SENSEX";
            return i;

        }
    }
}
