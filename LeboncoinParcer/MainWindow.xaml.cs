using LeboncoinParcer;
using SQLiteAspNetCoreDemo;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace LeboncoinParser
{
    public partial class MainWindow : Window
    {
        public static object Locker { get; } = new object();
        public MainWindow()
        {
            try
            {
                DataBase.CreateDB();
                Parser.Start();
                InitializeComponent();
            }
            catch (Exception exc)
            {
                exc.Write(Locker);
            }
        }
    }
}