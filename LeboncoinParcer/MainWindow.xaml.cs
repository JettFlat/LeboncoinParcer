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
        //public VM VM { get; set; }

        //public MainWindow(VM vm)
        //{
        //    InitializeComponent();
        //}

        public MainWindow()
        {
            
            DataBase.CreateDB();
            InitializeComponent();
            //LeboncoinParcer.Parser.Start();
        }
    }
}