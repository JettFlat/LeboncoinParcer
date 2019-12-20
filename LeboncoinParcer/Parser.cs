﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SQLiteAspNetCoreDemo;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace LeboncoinParcer
{
    public class Test
    {
        public static void Testing()
        {
        }
    }
    class Parser
    {
        public static int Timespan = 4000;
        public static object clocker = new object();
        public static ProxyContainer ProxyContainer { get; set; } = new ProxyContainer(new ObservableCollection<CustomWebProxy>(ProxyData.GetProxy(File.ReadAllLines("ProxyListEdited.pl")).ToList()));
        public static void Start()
        {
            ProxyContainer.Allbaned += ProxyContainer_Allbaned;
            //var linkpages = GetAllPages();
            //List<string> linkpages = new List<string> { };
            //BinaryFormatter formatter = new BinaryFormatter();
            ////using (FileStream fs = new FileStream("pages.ser", FileMode.OpenOrCreate))
            ////{
            ////    formatter.Serialize(fs, linkpages);
            ////}
            //using (FileStream fs = new FileStream("pages.ser", FileMode.OpenOrCreate))
            //{
            //    linkpages = (List<string>)formatter.Deserialize(fs);
            //}
            //List<string> RealtysUrls = new List<string> { };
            //foreach (var o in linkpages)
            //{
            //    var items = (ParseRealtyUrl(o));
            //    RealtysUrls.AddRange(items);
            //}
            //var d = GetDic(RealtysUrls);
            //SQLiteDBContext.AddToDb(d);
            SQLiteDBContext.ParseAd();


            //ParseAd(File.ReadAllText(@"D:\WORK\LeboncoinParcer\Test\Adpage2.html"));
            ////var linkpages = new List<string> { };
            ////foreach (var o in linkpages)
            ////    WritePages(ParseRealtyUrl(o));
        }
        public static Realty ParseRealty(Realty R, string html)
        {//TODO улучшить
            try
            {
                if (!string.IsNullOrWhiteSpace(R.Url) && !string.IsNullOrWhiteSpace(R.Id))
                {
                    var realty = new Realty();
                    realty.Id = R.Id;
                    realty.Url = R.Url;
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    string date = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_date']")?.InnerText ?? null;
                    if (date != null)
                    {
                        date = date.Replace('h', ':');
                        date = date.Replace("à", "");
                        DateTime dt = new DateTime();
                        //TODO add DataCheck
                        dt = DateTime.ParseExact(date, "dd/MM/yyyy  HH:mm", System.Globalization.CultureInfo.InvariantCulture); //TODO try cath
                        realty.Date = dt;
                    }
                    realty.Name = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_title']")?.FirstChild?.FirstChild?.InnerText ?? null;
                    realty.LocalisationTown = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_location_informations']")?.FirstChild?.InnerText ?? null;
                    realty.Type = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_real_estate_type']")?.FirstChild?.LastChild?.InnerText ?? null;
                    string RoomText = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_rooms']")?.FirstChild?.LastChild?.InnerText ?? null;
                    int c;
                    if (Int32.TryParse(RoomText, out c))
                        realty.Rooms = c;
                    realty.Surface = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_square']")?.FirstChild?.LastChild?.InnerText ?? null;
                    realty.Furniture = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_furnished']")?.FirstChild?.LastChild?.InnerText ?? null;
                    var ges = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_ges']")?.FirstChild?.LastChild?.FirstChild ?? null;
                    if (ges != null)
                    {
                        if (ges.ChildNodes.Count > 0)
                            realty.Ges = ges.ChildNodes.FirstOrDefault(x => x.Attributes.Any(y => y.DeEntitizeValue.Contains("_1sd0z"))).InnerText;
                        else
                            realty.Ges = ges.InnerText;
                    }
                    var eclass = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_energy_rate']")?.FirstChild?.LastChild?.FirstChild ?? null;
                    if (eclass != null)
                    {
                        if (eclass.ChildNodes.Count > 0)
                            realty.EnergyClass = eclass.ChildNodes.FirstOrDefault(x => x.Attributes.Any(y => y.DeEntitizeValue.Contains("_1sd0z"))).InnerText;
                        else
                            realty.EnergyClass = eclass.InnerText;
                    }
                    realty.Desciption = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_description_container']")?.FirstChild?.InnerText ?? null;
                    return realty;
                }
            }
            catch (Exception exc)
            {

            }
            return null;
        }
        static Realty ParseAd(string html)
        {//TODO Улучшить
            var realty = new Realty();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            string date = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_date']").InnerText;//.ParentNode.Attributes.Where(x => x.Name == "href").FirstOrDefault().DeEntitizeValue;
            date = date.Replace('h', ':');
            date = date.Replace("à", "");
            DateTime dt = new DateTime();
            dt = DateTime.ParseExact(date, "dd/MM/yyyy  HH:mm", System.Globalization.CultureInfo.InvariantCulture); //TODO try cath
            realty.Date = dt;
            realty.Name = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_title']").FirstChild.FirstChild.InnerText;
            realty.LocalisationTown = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_location_informations']").FirstChild.InnerText;
            realty.Type = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_real_estate_type']").FirstChild.LastChild.InnerText;
            realty.Rooms = Int32.Parse(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_rooms']").FirstChild.LastChild.InnerText);
            realty.Surface = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_square']").FirstChild.LastChild.InnerText;
            realty.Furniture = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_furnished']").FirstChild.LastChild.InnerText;
            realty.Ges = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_ges']").FirstChild.LastChild.FirstChild.ChildNodes.FirstOrDefault(x => x.Attributes.Any(y => y.DeEntitizeValue.Contains("_1sd0z"))).InnerText;
            realty.EnergyClass = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_energy_rate']").FirstChild.LastChild.FirstChild.ChildNodes.FirstOrDefault(x => x.Attributes.Any(y => y.DeEntitizeValue.Contains("_1sd0z"))).InnerText;
            realty.Desciption = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_description_container']").FirstChild.InnerText;
            return realty;
        }
        public static List<string> GetAllPages()
        {
            List<string> Parsed = new List<string> { };
            string url = "https://www.leboncoin.fr";
            string path = "/recherche/?category=10&owner_type=private&real_estate_type=1";
            int count = 1;
            while (path != null)
            {
                string page = null;
                while (page == null)
                    page = GetPage(url + path, Timespan);
                Parsed.Add(page);//File.WriteAllText($@"pages/{count}.html", page);
                path = Parse(page);
                count++;
            }
            return Parsed;
        }
        public static List<string> ParseRealtyUrl(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            try
            {
                var elements = htmlDoc.DocumentNode.SelectNodes("//a[@class='clearfix trackable']");
                return elements.Select(x => x.Attributes["href"]).Select(y => $"https://www.leboncoin.fr{y.DeEntitizeValue}").ToList();
            }
            catch (Exception) { }
            return null;
        }
        public static Dictionary<string, string> GetDic(List<string> UrlContainer)
        {
            Dictionary<string, string> results = new Dictionary<string, string> { };
            foreach (var o in UrlContainer)
            {
                string key = System.Text.RegularExpressions.Regex.Replace(o, @"[^\d]+", "");
                if (!results.ContainsKey(key))
                    results.Add(key, o);
            }
            return results;
        }
        public static Dictionary<string, string> GetPages(List<string> UrlContainer)
        {
            Dictionary<string, string> results = new Dictionary<string, string> { };
            Parallel.ForEach(UrlContainer, o =>
            {
                string page = null;
                while (page == null)
                    page = GetPage(o, Timespan);
                if (page == "skip")
                    return;
                results.Add(System.Text.RegularExpressions.Regex.Replace(o, @"[^\d]+", ""), page);
            });
            return results;
        }

        static void WritePages(List<string> UrlContainer, string path = @"pages/Realty/")
        {
            new DirectoryInfo(path).Create();
            Parallel.ForEach(UrlContainer, o =>
            {
                string page = null;
                while (page == null)
                    page = GetPage(o, Timespan);
                if (page == "skip")
                    return;
                File.WriteAllText($"{path}{System.Text.RegularExpressions.Regex.Replace(o, @"[^\d]+", "")}.html", page);
            });
        }
        private static void ProxyContainer_Allbaned()
        {
            throw new Exception("All Proxies banned");
        }
        public static string GetPage(string url, int Sleepms = 0)
        {
            //UseLOCKER
            if (Sleepms > 0)
                System.Threading.Thread.Sleep(Sleepms);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var p = new CustomWebProxy();
            bool wait = true;
            while (wait)
            {
                lock (clocker)
                {
                    if (ProxyContainer.Collections.Count > 0)
                    {
                        p = ProxyContainer.Collections.Cut();
                        request.Proxy = p;
                        wait = false;
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
            request.CookieContainer = new CookieContainer();
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            request.Headers.Add("accept-encoding: gzip, deflate, br");
            request.Headers.Add("accept-language: en-US,en;q=0.9,ru;q=0.8");
            request.Headers.Add("Cache-Control: no-cache");
            request.Host = "www.leboncoin.fr";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
            HttpWebResponse response = new HttpWebResponse();
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception exc)
            {
                lock (clocker)
                    ProxyContainer.Collections.Add(p);
                if (exc.Message == "The remote server returned an error: (410) Gone.")
                {
                    return "skip";
                }
                p.IsBanned = true;
                return null;
            }
            lock (clocker)
                ProxyContainer.Collections.Add(p);//Возможно прокси разбанят так что добавить isbanned=false
            string page = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream receiveStream = response.GetResponseStream())
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(receiveStream);
                    page = doc.Text;
                    return page;
                }
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
                p.IsBanned = true;//TODO check Proxycontainer ban values
            response.Close();
            return null;
        }
        public static string Parse(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            try
            {
                var path = htmlDoc.DocumentNode.SelectSingleNode("//span[@name='chevronright']").ParentNode.Attributes.Where(x => x.Name == "href").FirstOrDefault().DeEntitizeValue;
                return path;
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static bool IsBlocked(WebProxy proxy = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.leboncoin.fr");
            if (proxy != null)
                request.Proxy = proxy;
            request.CookieContainer = new CookieContainer();
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            request.Headers.Add("accept-encoding: gzip, deflate, br");
            request.Headers.Add("accept-language: en-US,en;q=0.9,ru;q=0.8");
            request.Headers.Add("Cache-Control: no-cache");
            request.Host = "www.leboncoin.fr";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
            HttpWebResponse response = new HttpWebResponse();
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException exc)
            {
                if (exc.Message == "The remote server returned an error: (403) Forbidden.")
                    return true;
            }
            string page = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream receiveStream = response.GetResponseStream())
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(receiveStream);
                    page = doc.Text;
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(page);
                    var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
                    if (node.OuterHtml != @"<title>You have been blocked</title>")//TODO Check
                        return false;
                }
            }
            return true;
        }
        static string GetCookies(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            StringBuilder st = new StringBuilder();
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                foreach (System.Net.Cookie cook in response.Cookies)
                {
                    st.AppendLine("Cookie:");
                    st.AppendLine($"{cook.Name} = {cook.Value}");
                    st.AppendLine($"Domain: {cook.Domain}");
                    st.AppendLine($"Path: {cook.Path}");
                    st.AppendLine($"Port: {cook.Port}");
                    st.AppendLine($"Secure: {cook.Secure}");
                    st.AppendLine($"When issued: {cook.TimeStamp}");
                    st.AppendLine($"Expires: {cook.Expires} (expired? {cook.Expired})");
                    st.AppendLine($"Don't save: {cook.Discard}");
                    st.AppendLine($"Comment: {cook.Comment}");
                    st.AppendLine($"Uri for comments: {cook.CommentUri}");
                    st.AppendLine($"Version: RFC {(cook.Version == 1 ? 2109 : 2965)}");
                    st.AppendLine($"String: {cook}");
                }
            }
            return st.ToString();
        }
    }
    class ProxyData
    {
        public string Adress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public static bool CheckProxy(string proxy)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://google.com");
            request.Proxy = new WebProxy(proxy);
            request.Timeout = 10000;
            request.Method = "HEAD";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return true;
                }
            }
            catch (Exception) { }
            return false;
        }
        public static bool Baning(string url, string adress, string user = null, string password = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (user != null && password != null)
                request.Proxy = new WebProxy(adress, false, null, new NetworkCredential(user, password));
            else
                request.Proxy = new WebProxy(adress);
            request.Timeout = 10000;
            request.CookieContainer = new CookieContainer();
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            request.Headers.Add("accept-encoding: gzip, deflate, br");
            request.Headers.Add("accept-language: en-US,en;q=0.9,ru;q=0.8");
            request.Headers.Add("Cache-Control: no-cache");
            request.Host = "www.leboncoin.fr";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return false;
                }
            }
            catch (Exception)
            {
            }
            return true;
        }
        public static bool CheckProxy(string url, string adress, string user = null, string password = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (user != null && password != null)
                request.Proxy = new WebProxy(adress, false, null, new NetworkCredential(user, password));
            else
                request.Proxy = new WebProxy(adress);
            request.Timeout = 10000;
            request.Method = "HEAD";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return true;
                }
            }
            catch (Exception) { }
            return false;
        }
        public static IEnumerable<CustomWebProxy> GetProxy(IEnumerable<string> ProxyList)
        {
            ProxyList = ProxyList.Where(x => !string.IsNullOrWhiteSpace(x));
            object locker = new object();
            List<ProxyData> Proxs = new List<ProxyData> { };
            Parallel.ForEach(ProxyList, o =>
            {
                if (!o.Contains('#'))
                {
                    lock (locker)
                        Proxs.Add(new ProxyData { Adress = o });
                    return;
                }
                string[] array = o.Split('#');//TODO проверить если нет юзера и пароля
                if (array.Length > 2)
                    lock (locker)
                        Proxs.Add(new ProxyData { Adress = array[0], UserName = array[1], Password = array[2] });
            }
            );
            foreach (var item in Proxs)
            {
                if (item.UserName != null && item.Password != null)
                    yield return new CustomWebProxy(item.Adress, false, null, new NetworkCredential(item.UserName, item.Password));
                else
                    yield return new CustomWebProxy(item.Adress);
            }
        }
        /// <summary>
        /// Get avalible http proxy 
        /// </summary>
        /// <param name="URL">Testing url</param>
        /// <param name="ProxyList">Proxy Line Format adress:port#username#pass</param>
        /// <returns></returns>
        public static IEnumerable<CustomWebProxy> GetAvalibleProxy(string URL, IEnumerable<string> ProxyList, int MaxDegreeParallelism = 4)
        {
            ProxyList = ProxyList.Where(x => !string.IsNullOrWhiteSpace(x));
            object locker = new object();
            List<ProxyData> Proxs = new List<ProxyData> { };
            Parallel.ForEach(ProxyList, o =>
            {
                if (!o.Contains('#'))
                {
                    lock (locker)
                        Proxs.Add(new ProxyData { Adress = o });
                    return;
                }
                string[] array = o.Split('#');//TODO проверить если нет юзера и пароля
                if (array.Length > 2)
                    lock (locker)
                        Proxs.Add(new ProxyData { Adress = array[0], UserName = array[1], Password = array[2] });
            }
            );
            foreach (var item in Proxs)
            {
                if (CheckProxy(URL, item.Adress, item.UserName, item.Password))
                {
                    if (item.UserName != null && item.Password != null)
                        yield return new CustomWebProxy(item.Adress, false, null, new NetworkCredential(item.UserName, item.Password));
                    else
                        yield return new CustomWebProxy(item.Adress);
                }
                else
                {

                }
            }
        }
    }
    public class CustomWebProxy : WebProxy
    {
        public bool IsBanned { get; set; } = false;
        //public CustomWebProxy(WebProxy proxy)
        //{
        //    this.Address = proxy.Address;
        //    this.BypassArrayList = proxy.BypassArrayList;
        //    this.BypassList = proxy.BypassList;
        //    this.BypassProxyOnLocal = proxy.BypassProxyOnLocal;
        //    this.Credentials = proxy.Credentials;
        //    this.UseDefaultCredentials = proxy.UseDefaultCredentials;
        //}
        public CustomWebProxy(bool IsBanned) : base()
        {
            this.IsBanned = IsBanned;
        }

        public CustomWebProxy() : base()
        {
        }

        protected CustomWebProxy(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public CustomWebProxy(string Address) : base(Address)
        {
        }

        public CustomWebProxy(string Address, bool BypassOnLocal) : base(Address, BypassOnLocal)
        {
        }

        public CustomWebProxy(string Address, bool BypassOnLocal, string[] BypassList) : base(Address, BypassOnLocal, BypassList)
        {
        }

        public CustomWebProxy(string Address, bool BypassOnLocal, string[] BypassList, ICredentials Credentials) : base(Address, BypassOnLocal, BypassList, Credentials)
        {
        }

        public CustomWebProxy(string Host, int Port) : base(Host, Port)
        {
        }

        public CustomWebProxy(Uri Address) : base(Address)
        {
        }

        public CustomWebProxy(Uri Address, bool BypassOnLocal) : base(Address, BypassOnLocal)
        {
        }

        public CustomWebProxy(Uri Address, bool BypassOnLocal, string[] BypassList) : base(Address, BypassOnLocal, BypassList)
        {
        }

        public CustomWebProxy(Uri Address, bool BypassOnLocal, string[] BypassList, ICredentials Credentials) : base(Address, BypassOnLocal, BypassList, Credentials)
        {
        }
    }
    public class ProxyContainer
    {//TODO CHECK WORK
        public ObservableCollection<CustomWebProxy> Collections { get; set; }
        public int StartCount { get; }
        //public object locker { get; } = new object();
        public delegate void MethodContainer();
        public event MethodContainer Allbaned;
        public ProxyContainer(ObservableCollection<CustomWebProxy> proxies)
        {
            Collections = new ObservableCollection<CustomWebProxy>(proxies);
            StartCount = Collections.Count;
            Collections.CollectionChanged += Collections_CollectionChanged;
        }
        private void Collections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {//TODO Use locker?
            var list = sender as ObservableCollection<CustomWebProxy>;
            if (list.Count == StartCount)
                if (!list.Any(x => x.IsBanned == false))
                    Allbaned();
            //if (list.Count < 1)
            //    while (list.Count < 1)
            //        System.Threading.Thread.Sleep(1000);
        }
    }



}
