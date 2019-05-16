using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public static class GeneralHelper
    {
        public static List<KeyValuePair<int, string>> GetKVPCollectionFromEnum(Type t)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            if (t.IsEnum)
            {
                var values = Enum.GetValues(t);
                foreach (var v in values)
                {
                    result.Add(new KeyValuePair<int, string>((int)v, Enum.GetName(t, v)));
                }
            }
            return result;
        }

        public static List<KeyValuePair<int, string>> GetKVPCollection(List<Segment> segments)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();

            foreach (var s in segments)
            {
                result.Add(new KeyValuePair<int, string>(s.SegmentId, s.SegmentName));
            }

            return result;
        }

        public static List<KeyValuePair<int, string>> GetKVPCollection(List<Portfolio> portfolios)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();

            foreach (var p in portfolios)
            {
                result.Add(new KeyValuePair<int, string>(p.PortfolioId, p.PortfolioName));
            }

            return result;
        }

        public static List<KeyValuePair<int, string>> GetKVPCollection(List<Stock> stocks)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();

            foreach (var s in stocks)
            {
                result.Add(new KeyValuePair<int, string>(s.StockId, s.StockName));
            }

            return result;
        }

    }
}
