using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Controls;
using HtmlAgilityPack;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace LeboncoinParcer
{
    class Parser
    {
        public static ProxyContainer ProxyContainer => new ProxyContainer(new ObservableCollection<CustomWebProxy>(ProxyData.GetAvalibleProxy("https://www.google.com", File.ReadAllLines("ProxyListEdited.pl")).ToList()));
        public static void Start()
        {
            try
            {
                ProxyContainer.Allbaned += ProxyContainer_Allbaned;
                GetPage("https://www.leboncoin.fr/recherche/?category=10&owner_type=private&real_estate_type=1");
                //var test = new ObservableCollection<WebProxy>(Proxies);
                //Task.Run(() =>
                //{
                //    System.Threading.Thread.Sleep(5000);
                //    Proxies.Add(test[0]);
                //    Proxies.Add(test[1]);
                //}
                //);
                //int col = Proxies.Count;
                //for (int i = 0; i < col; i++)
                //    Proxies.RemoveAt(0);
                //IsBlocked();
                //var spisok = ProxyData.GetAvalibleProxy("https://google.com", File.ReadAllLines("ProxyListEdited.pl")).ToList();
                //var test = CheckProxy("https://google.com/", "94.23.183.169:7951", "igp1091139", "1DraM7lfNS");
                //GetResp("https://www.leboncoin.fr/recherche/?category=10&owner_type=private&real_estate_type=1");
            }
            catch (Exception exc)
            {

            }
        }

        private static void ProxyContainer_Allbaned()
        {
            throw new Exception("All Proxies banned");
        }

        public static string GetPage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var p = ProxyContainer.Collections.Cut(); //TODO check ProxyContainer Collection cont
            request.Proxy = p;
            request.CookieContainer = new CookieContainer();
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            request.Headers.Add("accept-encoding: gzip, deflate, br");
            request.Headers.Add("accept-language: en-US,en;q=0.9,ru;q=0.8");
            request.Headers.Add("Cache-Control: no-cache");
            request.Host = "www.leboncoin.fr";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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
                p.IsBanned = true;
            response.Close();
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
        public bool IsAllBaned
        {
            get
            {
                bool val = !Collections.Any(x => x.IsBanned == false);
                if (val == true)
                    Allbaned();
                return val;
            }
        }
        public delegate void MethodContainer();
        public event MethodContainer Allbaned;
        public ProxyContainer(ObservableCollection<CustomWebProxy> proxies)
        {
            Collections = proxies;
            Collections.CollectionChanged += Collections_CollectionChanged;
        }

        private void Collections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var list = sender as ObservableCollection<WebProxy>;
            if (list.Count < 1)
            {
                while (list.Count < 1)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }



}
