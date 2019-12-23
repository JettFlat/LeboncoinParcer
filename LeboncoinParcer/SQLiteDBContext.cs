using LeboncoinParcer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SQLiteAspNetCoreDemo
{
    public class SQLiteDBContext : DbContext
    {
        public DbSet<Realty> Realtys { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=DataBase.db");


    }
    public class DataBase
    {
        public delegate void MethodContainer();
        public static event MethodContainer DBUpdated;
        public static List<Realty> Get()
        {
            using (var context = new SQLiteDBContext())
            {
                var res = context.Realtys.Where(x => x.Name != null).ToList();
                return res;
            }
        }
        public static void AddToDb(Dictionary<string, string> dictionary)
        {
            if (dictionary.Count > 0)
            {
                using (var context = new SQLiteDBContext())
                {
                    foreach (var item in dictionary)
                    {
                        if (context.Realtys.Where(x => x.Id == item.Key).ToList().Count < 1)
                            context.Realtys.Add(new Realty { Id = item.Key, Url = item.Value });
                    }
                    context.SaveChanges();
                }
            }
        }
        //public static void ParseAd()
        //{
        //    //CreateDB();
        //    Parser.Log += "Starting parsing/updating all ads.".ToLogFormat();
        //    List<Realty> list = new List<Realty> { };
        //    int count = 0;
        //    using (var context = new SQLiteDBContext())
        //    {
        //        list = context.Realtys.ToList();
        //    }
        //    foreach (var o in list)
        //    {
        //        count++;
        //        try
        //        {
        //            using (var context = new SQLiteDBContext())
        //            {
        //                //Parser.Log += $"Parsing {o.Url} page".ToLogFormat();
        //                string page = null;
        //                while (page == null)
        //                    page = Parser.GetPage(o.Url, Parser.ProxyTimeout);
        //                var item = context.Realtys.FirstOrDefault(x => x.Id == o.Id);
        //                if (page == "skip")
        //                {
        //                    Parser.Log += $"Page {count}/{list.Count} broken {o.Url} ".ToLogFormat();
        //                    item.Isbroken = true;
        //                }
        //                else
        //                {
        //                    var parsed = Parser.ParseRealty(o, page);
        //                    item.Update(parsed);
        //                    Parser.Log += $"Page {count}/{list.Count} {o.Url} parsed".ToLogFormat();
        //                }
        //                if (string.IsNullOrWhiteSpace(item.Phone))
        //                {
        //                    if (!item.Isbroken)
        //                        Parser.Log += $"Empty phone page {count}/{list.Count} {o.Url} ".ToLogFormat();
        //                    context.Realtys.Remove(item);
        //                }
        //                context.SaveChanges();
        //                DBUpdated();
        //            }
        //        }
        //        catch (Exception)
        //        {

        //        }
        //        if (!Parser.IsRun)
        //            break;
        //    }
        ////}
        public static void ParseAd(CancellationToken token)
        {
            //CreateDB();
            Parser.Log += "Starting parsing/updating all ads.".ToLogFormat();
            List<Realty> list = new List<Realty> { };
            object locker = new object();
            int ccount = 0;
            using (var context = new SQLiteDBContext())
            {
                list = context.Realtys.ToList();
            }
            try
            {
                //foreach (var o in list)
                //{
                   
                //}

                Parallel.ForEach(list, new ParallelOptions { CancellationToken = token }, o =>
                   {
                       int count;
                       lock (locker)
                       {
                           ccount++;
                           count = ccount;
                       }
                       try
                       {
                           using (var context = new SQLiteDBContext())
                           {
                               //Parser.Log += $"Parsing {o.Url} page".ToLogFormat();
                               string page = null;
                               while (page == null)
                                   page = Parser.GetPage(o.Url, Parser.ProxyTimeout);
                               var item = context.Realtys.FirstOrDefault(x => x.Id == o.Id);
                               if (page == "skip")
                               {
                                   Parser.Log += $"Page {count}/{list.Count} broken {o.Url} ".ToLogFormat();
                                   item.Isbroken = true;
                               }
                               else
                               {
                                   var parsed = Parser.ParseRealty(o, page);
                                   item.Update(parsed);
                                   Parser.Log += $"Page {count}/{list.Count} {o.Url} parsed".ToLogFormat();
                               }
                               if (string.IsNullOrWhiteSpace(item.Phone) || item.Phone == "Empty")
                               {
                                   if (!item.Isbroken)
                                       Parser.Log += $"Empty phone page {count}/{list.Count} {o.Url} ".ToLogFormat();
                                   context.Realtys.Remove(item);
                               }
                               context.SaveChanges();
                               DBUpdated();
                           }
                       }
                       catch (Exception)
                       {

                       }
                   });
            }
            catch (OperationCanceledException exc) { }
        }
        public static void CreateDB()
        {
            using (var context = new SQLiteDBContext())
            {
                context.Database.EnsureCreated();
                context.SaveChanges();
            }
        }
    }
}
