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
        const string baseUrl = @"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={0}.{1}&apikey=";

        string[] keys = new string[3] { "8P8RIZLF7NMHJZO9", "F7YMQ8K4LSQLW0SQ", "767ABZ44DXOXQXY7" };



        public int num = 0;


        public async Task<Stock> GetOnlineStockDetails(Stock stock)
        {
            try
            {


                //Random r = new Random();
                //int num = r.Next(0, 3);



                string topUpUrl = baseUrl + keys[num];
                num++;
                if (num == 3) num = 0;
                string response = await Task.Run(() => WebHelper.GetWebResponseAsync(string.Format(baseUrl, stock.Code.Code, stock.StockExchange == Exchange.NSE ? "NS" : "BSE")));
                //string response = WebHelper.GetWebResponse(string.Format(topUpUrl, stock.Code.Code, "NS")); // Exchange.NSE ? "NS" : "BSE"));
                JObject res = JObject.Parse(response);

                stock.DayHigh = res.First.First.First.Next.Next.First.ToDecimal();
                stock.DayLow = res.First.First.First.Next.Next.Next.First.ToDecimal();
                stock.CMP = res.First.First.First.Next.Next.Next.Next.First.ToDecimal();
                stock.Close = res.First.First.First.Next.Next.Next.Next.Next.Next.Next.First.ToDecimal();
                stock.Change = stock.CMP - stock.Close;



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return stock;
        }


        public Task<decimal> GetStockPrice(string symbol, bool IsNSE)
        {
            throw new NotImplementedException();
        }

        public async Task<Index> GetIndexPoints(bool isNifty = true)
        {
            var i = await Task.Run(() => GetIndex());
            return i;
        }

        Index GetIndex()
        {
            Index i = new Index();
            i.IndexName = "NIFTY";
            return i;
        }
    }
}
