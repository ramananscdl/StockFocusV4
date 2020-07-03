using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StockFocus.Helper
{
    public static class WebHelper
    {

        public static async Task<string> GetWebResponseAsync(string url)
        {
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = await Task.Run(() => myReq.GetResponse());
            Stream responseStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(responseStream, encode);

            return readStream.ReadToEnd();
        }



        public static string GetWebResponse(string url)
        {


            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);

            WebResponse response = myReq.GetResponse();
            Stream responseStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(responseStream, encode);

            return readStream.ReadToEnd();
        }

    }
}
