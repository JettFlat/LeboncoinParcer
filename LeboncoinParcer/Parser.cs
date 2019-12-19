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

namespace LeboncoinParcer
{
    public class Test
    {
        public static void Testing()
        {
            var list = Parser.ParseRealtyUrl(File.ReadAllText(@"D:\WORK\Backup\pages\93.html"));
            string rt = "https://www.leboncoin.fr/locations/1724535725.htm/";
            rt = System.Text.RegularExpressions.Regex.Replace(rt, @"[^\d]+", "");
            //using (var context = new SQLiteDBContext())
            //{
            //    context.Realtys.Add(new Realty { Id = 2344, Date = DateTime.Now });
            //    //var test = context.Realtys;
            //    context.SaveChanges();
            //}
        }
    }
    class Parser
    {
        public static object clocker = new object();
        public static ProxyContainer ProxyContainer { get; set; } = new ProxyContainer(new ObservableCollection<CustomWebProxy>(ProxyData.GetProxy(File.ReadAllLines("ProxyListEdited.pl")).ToList()));
        public static void Start()
        {
            ProxyContainer.Allbaned += ProxyContainer_Allbaned;
            var linkpages = GetAllPages();
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("pages.ser", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, linkpages);
            }
            //using (FileStream fs = new FileStream("pages.ser", FileMode.OpenOrCreate))
            //{
            //    List<string> deserilize = (List<string>)formatter.Deserialize(fs);
            //}
            foreach (var o in linkpages)
                WritePages(ParseRealtyUrl(o));
            //Parallel.ForEach(linkpages, o => {
            //    WritePages(ParseRealtyUrl(o));
            //});
            //var list = Parser.ParseRealtyUrl(File.ReadAllText(@"D:\WORK\Backup\pages\93.html"));
            //WritePages(list);
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
                    page = GetPage(url + path);
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
        public static List<string> GetPages(List<string> UrlContainer)
        {
            List<string> results = new List<string> { };
            Parallel.ForEach(UrlContainer, o =>
            {
                results.Add(GetPage(o));
            });
            return results;
        }
        static void WritePages(List<string> UrlContainer, string path = @"pages/Realty/")
        {
            new DirectoryInfo(path).Create();
            Parallel.ForEach(UrlContainer, o =>
            {
                File.WriteAllText($"{path}{System.Text.RegularExpressions.Regex.Replace(o, @"[^\d]+", "")}.html", GetPage(o));
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
            lock (clocker)
            {
                p = ProxyContainer.Collections.Cut();
                request.Proxy = p;
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
            catch (Exception)
            {
                p.IsBanned = true;
                lock (clocker)
                    ProxyContainer.Collections.Add(p);
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
        public static string GetCookies(string url)
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
            if (list.Count < 1)
                while (list.Count < 1)
                    System.Threading.Thread.Sleep(1000);
        }
    }



}
