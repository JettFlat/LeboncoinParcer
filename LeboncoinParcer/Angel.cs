//using AngleSharp;
//using AngleSharp.Io;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;

//namespace LeboncoinParcer
//{
//    class Angel
//    {
//        public static async System.Threading.Tasks.Task TestAsync()
//        {

//            try
//            {
//                //string padress = "51.158.68.133:8811";
//                //if (!Parser.CheckProxy(padress))
//                //    throw new Exception("Proxy not work");
//                //System.Net.WebProxy proxy = new System.Net.WebProxy(padress);
//                var handler = new HttpClientHandler()
//                {
//                    //Proxy = proxy,
//                    PreAuthenticate = true,
//                    UseDefaultCredentials = false,
//                };
//                var requester = new DefaultHttpRequester();
//                //requester.Headers["Accept"] = "*/*";
//                //requester.Headers["Referer"] = "https://www.leboncoin.fr/recherche/?category=10&owner_type=private&real_estate_type=1";
//                //requester.Headers["Accept-Language"] = "en-US,ru;q=0.8,en-CA;q=0.5,en;q=0.3";
//                //requester.Headers["Origin"] = "https://www.leboncoin.fr";
//                //requester.Headers["Accept-Encoding"] = "gzip, deflate";
//                requester.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
//                //requester.Headers["Host"] = "ams1-ib.adnxs.com";
//                //requester.Headers["Connection"] = "Keep-Alive";
//                var config = Configuration.Default.WithRequesters(handler).WithDefaultLoader().WithTemporaryCookies().WithJs().With(requester).WithCss().WithCookies();
//                var address = "https://www.whatismybrowser.com/";//"https://www.leboncoin.fr/recherche/?category=10&owner_type=private&real_estate_type=1";
//                var context = BrowsingContext.New(config);
//                var document = await context.OpenAsync(address);
//                var page = document.DocumentElement.OuterHtml;
//            }
//            catch (Exception exc)
//            {

//            }
//        }
//    }
//}
