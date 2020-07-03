using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockFocus.Helper;
using StockFocus.Entites;
using Newtonsoft.Json.Linq;

namespace StockFocus.Service
{
    public class NSEStockService : IStockService
    {

        const string baseUrl = @"https://nseindia.com/live_market/dynaContent/live_watch/get_quote/GetQuote.jsp?symbol=";

        const string homeUrl = @"https://nseindia.com/index_nse.htm";

        public async Task<Stock> GetOnlineStockDetails(Stock stock)
        {
            try
            {
                string json = await Task.Run(() => ExtractJson(WebHelper.GetWebResponse(baseUrl + stock.NSECode)));
                var baseObject = JObject.Parse(json)["data"][0];
                stock.Change = baseObject["change"].ToDecimal();
                stock.Close = baseObject["previousClose"].ToDecimal();
                stock.CMP = baseObject["lastPrice"].ToDecimal();
                stock.DayHigh = baseObject["dayHigh"].ToDecimal();
                stock.DayLow = baseObject["dayLow"].ToDecimal();
                stock.StockName = baseObject["companyName"].ToString();
                stock.YearHigh = baseObject["high52"].ToDecimal();
                stock.YearLow = baseObject["low52"].ToDecimal();
            }
            catch (Exception ex)
            {
                //stock.StockName =  stock.StockName+" Not available or Error";
                /////TODO: Handle this error 
            }
            return stock;
        }

        private static string ExtractJson(string json)
        {
            json = json.Substring(json.IndexOf("{\"tradedDate\"") - 1);
            json = json.Substring(0, json.IndexOf("lastUpdateTime") + 45);
            return json;
        }

        public async Task<decimal> GetStockPrice(string symbol, bool IsNSE = true)
        {
            try
            {
                string json = await Task.Run(() => ExtractJson(WebHelper.GetWebResponse(baseUrl + symbol)));

                var baseObject = JObject.Parse(json)["data"][0];


                return baseObject["lastPrice"].ToDecimal();
            }
            catch (Exception)
            {


                return "0.00".ToDecimal();
            }


        }

        Index GetNiftyPoints()
        {
            Index i = new Index();
            decimal cmp = 0;
            i.IndexName = "NIFTY";
            string content = WebHelper.GetWebResponse(homeUrl);
            content = content.Substring(content.IndexOf("lastPriceNIFTY 50")); content = content.Substring(150);
            content = content.Substring(content.IndexOf("lastPriceNIFTY 50"));
            content = content.Substring(content.IndexOf(">"));
            content = content.Substring(content.IndexOf(">"));
            content = content.Substring(1, content.IndexOf("<"));
            decimal.TryParse(content, out cmp);
            i.CMP = cmp;
            return i;
        }

        public async Task<Index> GetIndexPoints(bool isNifty = true)
        {
            var i = await Task.Run(() => GetNiftyPoints());
            return i;
        }


    }
}
