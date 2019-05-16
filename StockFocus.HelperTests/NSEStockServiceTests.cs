using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockFocus.Entites;
using StockFocus.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Service.Tests
{
    [TestClass()]
    public class NSEStockServiceTests
    {
        [TestMethod()]
        public void NSEServiceTest()
        {

            fun();



        }

        async void fun()
        {
            string[] stocks = { "WABAG", "LT", "KGL", "TATAMOTORS", "BEPL", "RAIN" }; //"WABAG", "LT", "KGL", "TATAMOTORS", "INEOSSTYRO", "BEPL", "RAIN" 

            Stopwatch w = new Stopwatch();
            w.Start();

            NSEStockService service = new NSEStockService();
            /// service.GetStockPrice("KGL");
            /// 
            List<Stock> tasks = new List<Stock>();
            foreach (var stk in stocks)
            {
                Stock s = new Stock() { NSECode = stk };
                var t = await Task<Stock>.Factory.StartNew(() => service.GetOnlineStockDetails(s).Result);
                tasks.Add(t);
            }
            //Task.WaitAll(tasks.ToArray());

            w.Stop();
            var totalTime = w.ElapsedMilliseconds;
        }

        [TestMethod()]
        public void NSEServiceTest1()
        {
            SQLDataLayer dl = new SQLDataLayer();
            var stocks = dl.GetStocksByPortfolioId(1);

        }

        [TestMethod()]
        public void NSEServiceTest2()
        {
            AlphaVantageService yss = new AlphaVantageService();
            Stock s = new Stock() { StockName = "526173", NSECode = "526173", StockExchange = Exchange.BSE };
            var ts = yss.GetOnlineStockDetails(s);

        }

        [TestMethod()]
        public void GetAllTransaction()
        {
            SQLDataLayer dl = new SQLDataLayer();
            var trs = dl.GetAllTransactionsByStockId(4, 1);
        }

        [TestMethod()]
        public void NiftyNSETest()
        {
            NSEStockService se = new NSEStockService();
            se.GetNiftyPoints();
        }
    }
}