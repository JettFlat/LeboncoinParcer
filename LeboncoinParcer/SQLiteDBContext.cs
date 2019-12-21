using LeboncoinParcer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SQLiteAspNetCoreDemo
{
    public class SQLiteDBContext : DbContext
    {
        public DbSet<Realty> Realtys { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=blogging.db");


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
        public static void ParseAd()
        {
            //CreateDB();
            Parser.Log += "Starting parsing/updating all ads.".ToLogFormat();
            List<Realty> list = new List<Realty> { };
            using (var context = new SQLiteDBContext())
            {
                list = context.Realtys.ToList();
            }
            foreach (var o in list)
            {
                try
                {
                    using (var context = new SQLiteDBContext())
                    {
                        string page = null;
                        while (page == null)
                            page = Parser.GetPage(o.Url, Parser.Timespan);
                        var item = context.Realtys.FirstOrDefault(x => x.Id == o.Id);
                        var parsed = Parser.ParseRealty(o, page);
                        item.Update(parsed);
                        if (string.IsNullOrWhiteSpace(item.Phone))
                            context.Realtys.Remove(item);
                        context.SaveChanges();
                        DBUpdated();
                    }
                }
                catch (Exception)
                {

                }
                if (!Parser.IsRun)
                    break;
            }
        }
        public static void CreateDB()
        {
            using (var context = new SQLiteDBContext())
            {
                context.Database.EnsureCreated();
                context.SaveChanges();
            }
        }
        //public static void ParseAd()
        //{
        //    List<Realty> list = new List<Realty> { };
        //    using (var context = new SQLiteDBContext())
        //    {
        //        list = context.Realtys.ToList();
        //    }
        //    foreach (var o in list)
        //    {
        //        try
        //        {
        //            using (var context = new SQLiteDBContext())
        //            {
        //                string page = null;
        //                while (page == null)
        //                    page = Parser.GetPage(o.Url, Parser.Timespan);
        //                var item = context.Realtys.FirstOrDefault(x => x.Id == o.Id);
        //                var parsed = Parser.ParseRealty(o, page);
        //                item.Update(parsed);
        //                context.SaveChanges();
        //                DBUpdated();
        //            }
        //        }
        //        catch (Exception)
        //        {

        //        }

        //    }
        //}
    }
}
