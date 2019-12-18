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
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace LeboncoinParcer
{
    class Parser
    {
        [Obsolete]
        public PhantomJSDriver PJS { get; set; }
        public PhantomJSOptions options { get; set; }
        public Proxy CurrentProxy { get; set; }

        [Obsolete]
        public Parser(bool EnableProxy)
        {
            options = new PhantomJSOptions();
            options.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
            var service = PhantomJSDriverService.CreateDefaultService();
            if (EnableProxy)
            {
                string padress = "51.91.212.159:3128";
                CurrentProxy = new Proxy();
                CurrentProxy.HttpProxy = string.Format(padress);
                service.ProxyType = "http";
                service.Proxy = CurrentProxy.HttpProxy;
            }
            service.LoadImages = false;
            service.HideCommandPromptWindow = true;
            PJS = new PhantomJSDriver(service, options);
            PJS.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
            PJS.Manage().Cookies.DeleteAllCookies();
        }
        [Obsolete]
        public static void Test()
        {
            try
            {//Todo Enable Proxy
                GetResp("https://www.leboncoin.fr/recherche/?category=10&owner_type=private&real_estate_type=1");

                //Parser pars1 = new Parser(false);
                //pars1.PJS.Navigate().GoToUrl(@"https://google.com");//https://www.leboncoin.fr/recherche/?category=10&owner_type=private&real_estate_type=1");
                //var cookies = pars1.PJS.Manage().Cookies.AllCookies;
                //(pars1.PJS as PhantomJSDriver).GetScreenshot().SaveAsFile("scren.png");
                //NextPage(pars1.PJS);
                //var html = (pars1.PJS as PhantomJSDriver).PageSource;
                //pars1.PJS.Quit();
                //File.WriteAllText("Page.html", html);
            }
            catch (Exception exc)
            {
            }
        }

        [Obsolete]
        public static void NextPage(PhantomJSDriver PJS)
        {
            var next = PJS.FindElementByName("chevronright").FindElement(By.XPath("./parent::*"));
            next.Click();
        }
       
        public static void GetResp(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = new CookieContainer();
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
                request.Host = "www.leboncoin.fr";
                request.Headers.Add("accept-encoding: gzip, deflate, br");
                request.Headers.Add("accept-language: en-US,en;q=0.9,ru;q=0.8");
                request.Headers.Add("Cache-Control: no-cache");
                request.Headers.Add("Postman-Token: acbce3eb-50ca-4b26-a043-9bd6cd5a042f");
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
                    }
                }
                response.Close();
            }
            catch (Exception excc)
            {

            }
        }
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
}
