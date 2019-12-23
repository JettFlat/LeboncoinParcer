using System;
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
using System.Threading;
using LeboncoinParser;
using System.Runtime.Serialization.Formatters.Binary;

namespace LeboncoinParcer
{
    class Parser
    {
        public static CancellationTokenSource cancelTokenSource { get; set; } = new CancellationTokenSource();
        public static CancellationToken Token { get; set; } = cancelTokenSource.Token;
        public static int ProxyTimeout { get; set; } = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json")).ProxyTimeout;
        public static object clocker { get; } = new object();
        public static object llocker = new object();
        public delegate void MethodContainer();
        public static event MethodContainer LogChanged;
        static string _log = "Start".ToLogFormat();
        public static string Log
        {
            get => _log;
            set
            {
                _log = value;
                LogChanged?.Invoke();
                try
                {
                    lock (llocker)
                        File.WriteAllText("Log.lg", _log);
                }
                catch (Exception) { }
            }
        }
        public static ProxyContainer ProxyContainer { get; set; } = new ProxyContainer(new ObservableCollection<CustomWebProxy>(ProxyData.GetProxy(File.ReadAllLines("Proxy.pl")).ToList()));
        public static void Stoptoken()
        {
            cancelTokenSource.Cancel();
        }
        public static void Resettoken()
        {
            cancelTokenSource.Dispose();
            cancelTokenSource = new CancellationTokenSource();
            Token = cancelTokenSource.Token;
        }
        public static void Start()
        {
            ProxyContainer.Allbaned += ProxyContainer_Allbaned;
            var linkpages = GetAllPages().ToList();
            #region Tests
            //var tewst = new Settings { ProxyTimeout = 4000, TableId = "1AKP9CPyQ468Z3QKMggbfB4UlpSbbQ9XSUm0Hl6j4gS4" };
            //File.WriteAllText("Settings.json", Newtonsoft.Json.JsonConvert.SerializeObject(tewst));
            //List<string> linkpages = new List<string> { };
            //BinaryFormatter formatter = new BinaryFormatter();
            //////////using (FileStream fs = new FileStream("pages.ser", FileMode.OpenOrCreate))
            //////////{
            //////////    formatter.Serialize(fs, linkpages);
            //////////}
            //using (FileStream fs = new FileStream("pages.ser", FileMode.OpenOrCreate))
            //{
            //    linkpages = (List<string>)formatter.Deserialize(fs);
            //}
            #endregion
            List<string> RealtysUrls = new List<string> { };
            foreach (var o in linkpages)
            {
                var items = ParseRealtyUrl(o);
                RealtysUrls.AddRange(items);
            }
            var d = GetDic(RealtysUrls);
            DataBase.AddToDb(d);
            UpdateDBitems(Token);
            
        }
        public static void Export()
        {
            LeboncoinParcer.Sheets.Export(DataBase.Get());
        }
        public static Realty ParseRealty(Realty R, string html)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(R.Url) && !string.IsNullOrWhiteSpace(R.Id))
                {
                    var realty = new Realty();
                    realty.Id = R.Id;
                    realty.Url = R.Url;
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    string date = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_date']")?.InnerText) ?? null;
                    if (date != null)
                    {
                        date = date.Replace('h', ':');
                        date = date.Replace("à", "");
                        DateTime dt = new DateTime();
                        dt = DateTime.ParseExact(date, "dd/MM/yyyy  HH:mm", System.Globalization.CultureInfo.InvariantCulture); //TODO try cath
                        realty.Date = dt;
                    }
                    string phone = null;
                    while (phone == null)
                        phone = GetPhone(R.Id, Parser.ProxyTimeout);
                    realty.Phone = phone;
                    realty.Name = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_title']")?.FirstChild?.FirstChild?.InnerText) ?? null;
                    realty.LocalisationTown = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_location_informations']")?.FirstChild?.InnerText) ?? null;
                    realty.Type = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_real_estate_type']")?.FirstChild?.LastChild?.InnerText) ?? null;
                    string RoomText = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_rooms']")?.FirstChild?.LastChild?.InnerText) ?? null;
                    int c;
                    if (Int32.TryParse(RoomText, out c))
                        realty.Rooms = c;
                    realty.Surface = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_square']")?.FirstChild?.LastChild?.InnerText) ?? null;
                    realty.Furniture = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_furnished']")?.FirstChild?.LastChild?.InnerText) ?? null;
                    var ges = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_ges']")?.FirstChild?.LastChild?.FirstChild ?? null;
                    if (ges != null)
                    {
                        if (ges.ChildNodes.Count > 0)
                            realty.Ges = HtmlEntity.DeEntitize(ges.ChildNodes.FirstOrDefault(x => x.Attributes.Any(y => y.DeEntitizeValue.Contains("_1sd0z")))?.InnerText ?? null);
                        else
                            realty.Ges = ges.InnerText;
                    }
                    var eclass = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='criteria_item_energy_rate']")?.FirstChild?.LastChild?.FirstChild ?? null;
                    if (eclass != null)
                    {
                        if (eclass.ChildNodes.Count > 0)
                            realty.EnergyClass = HtmlEntity.DeEntitize(eclass.ChildNodes.FirstOrDefault(x => x.Attributes.Any(y => y.DeEntitizeValue.Contains("_1sd0z")))?.InnerText ?? null);
                        else
                            realty.EnergyClass = eclass.InnerText;
                    }
                    realty.Desciption = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qa-id='adview_description_container']")?.FirstChild?.InnerText) ?? null;
                    return realty;
                }
            }
            catch (Exception exc)
            {

            }
            return null;
        }
        public static void UpdateDBitems(CancellationToken token)
        {
            DataBase.ParseAd(token);
            Parser.Log += "End of parsing".ToLogFormat();
        }
        public static IEnumerable<string> GetAllPages()
        {
            Log += "Getting ad links from search pages.".ToLogFormat();
            List<string> Parsed = new List<string> { };
            string url = "https://www.leboncoin.fr";
            string path = "/recherche/?category=10&owner_type=private&real_estate_type=1";
            int count = 1;
            while (path != null && !Token.IsCancellationRequested)
            {
                string page = null;
                while (page == null && !Token.IsCancellationRequested)
                    page = GetPage(url + path, ProxyTimeout);
                Parsed.Add(page);//File.WriteAllText($@"pages/{count}.html", page);
                yield return page;
                path = Parse(page);
                Log += $"Downloaded page#{count}".ToLogFormat();
                count++;
            }
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
        private static void ProxyContainer_Allbaned()
        {
            Parser.Log += "All proxies had been banned. Please change it and restart the application or wait. It could take a while.".ToLogFormat();
            //throw new Exception("All Proxies banned");
        }
        public static string GetPage(string url, int Sleepms = 0)
        {
            try
            {
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
                    if (exc.Message == "The remote server returned an error: (410) Gone.")
                    {
                        lock (clocker)
                            ProxyContainer.Collections.Add(p);
                        return "skip";
                    }
                    if (exc.Message.Contains("did not properly respond after a period of time") || exc.Message.Contains("An error occurred while sending the request"))
                    {
                        lock (clocker)
                            ProxyContainer.Collections.Add(p);
                        return null;
                    }
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
                    p.IsBanned = true;
                response.Close();
            }
            catch (Exception)
            { }
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
        public static string GetPhone(string ID, int Sleepms = 0, string key = "54bb0281238b45a03f0ee695f73e704f")
        {
            try
            {
                if (Sleepms > 0)
                    System.Threading.Thread.Sleep(Sleepms);
                string result = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.leboncoin.fr/api/utils/phonenumber.json");
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
                request.Method = "POST"; // для отправки используется метод Post
                request.CookieContainer = new CookieContainer();
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
                request.Accept = "application/json";
                request.Headers.Add("accept-encoding: gzip, deflate, br");
                request.Headers.Add("accept-language: en-US,en;q=0.9,ru;q=0.8");
                request.Host = "api.leboncoin.fr";
                request.Headers.Add("Cache-Control: no-cache");

                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
                string data = $"app_id=leboncoin_web_utils&key={key}&list_id={ID}&text=1";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
                request.ContentLength = byteArray.Length;

                //записываем данные в поток запроса
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                HttpWebResponse response = new HttpWebResponse();
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception exc)
                {
                    if (exc.Message == "The remote server returned an error: (403) Forbidden.")
                    {
                        p.IsBanned = true;
                        lock (clocker)
                            ProxyContainer.Collections.Add(p);
                        return null;
                    }
                    if (exc.Message.Contains("did not properly respond after a period of time") || exc.Message.Contains("An error occurred while sending the request"))
                    {
                        lock (clocker)
                            ProxyContainer.Collections.Add(p);
                        return null;
                    }
                    //else
                    //{
                    //    if (exc.Message != "The remote server returned an error: (410) Gone.")
                    //    {

                    //    }
                    //    else
                    //    {

                    //    }
                    //}
                    lock (clocker)
                        ProxyContainer.Collections.Add(p);
                    return "Empty";
                }
                lock (clocker)
                    ProxyContainer.Collections.Add(p);
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                        result = System.Text.RegularExpressions.Regex.Replace(result, @"[^\d]+", "");
                        return result;
                    }
                }
            }
            catch (Exception exc) { }
            return null;
        }
    }
    class ProxyData
    {
        public string Adress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
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
                string[] array = o.Split('#');
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
    {
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
        {
            var list = sender as ObservableCollection<CustomWebProxy>;
            if (list.Count == StartCount)
                if (!list.Any(x => x.IsBanned == false))
                    Allbaned?.Invoke();
        }
    }
    public class Settings
    {
        public int ProxyTimeout { get; set; }
        public string TableId { get; set; }
    }


}
