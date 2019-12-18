using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeboncoinParcer
{
    class Phantom
    {
        [Obsolete]
        public PhantomJSDriver PJS { get; set; }
        public PhantomJSOptions options { get; set; }
        public Proxy CurrentProxy { get; set; }

        [Obsolete]
        public Phantom(bool EnableProxy)
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
        public static void Test()
        {
            try
            {//Todo Enable Proxy
                #region PHantom
                //Parser pars1 = new Parser(false);
                //pars1.PJS.Navigate().GoToUrl(@"https://google.com");//https://www.leboncoin.fr/recherche/?category=10&owner_type=private&real_estate_type=1");
                //var cookies = pars1.PJS.Manage().Cookies.AllCookies;
                //(pars1.PJS as PhantomJSDriver).GetScreenshot().SaveAsFile("scren.png");
                //NextPage(pars1.PJS);
                //var html = (pars1.PJS as PhantomJSDriver).PageSource;
                //pars1.PJS.Quit();
                //File.WriteAllText("Page.html", html);
                #endregion
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



    }
}
