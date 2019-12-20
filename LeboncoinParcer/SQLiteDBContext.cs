using LeboncoinParcer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SQLiteAspNetCoreDemo
{
    public class SQLiteDBContext : DbContext
    {
        public DbSet<Realty> Realtys { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=blogging.db");
        public static void AddToDb(Dictionary<string, string> dictionary)
        {
            using (var context = new SQLiteDBContext())
            {
                foreach (var item in dictionary)
                    context.Realtys.Add(new Realty { Id = item.Key, Url = item.Value });
                context.SaveChanges();
            }
        }
        public static void ParseAd()
        {
            using (var context = new SQLiteDBContext())
            {
                foreach (var o in context.Realtys)
                {
                    string page = null;
                    while (page == null)
                        page = Parser.GetPage(o.Url, Parser.Timespan);
                    try
                    {
                        var item = context.Realtys.FirstOrDefault(x => x.Id == o.Id);
                        var test = Parser.ParseRealty(o, page);
                        item.Update(test);
                    }
                    catch (Exception)
                    {

                    }
                }
                context.SaveChanges();
            }
        }
        public static List<Realty> Get(int maxcount)
        {
            using (var context = new SQLiteDBContext())
            {
                var res = context.Realtys.Take(maxcount).ToList();
                return res;
            }
        }
        //public static void Update(Realty realty)
        //{
        //    using (var context = new SQLiteDBContext())
        //    {
        //            context.Realtys.Remove(new Realty());
        //        context.SaveChanges();
        //    }
        //}
    }
}
