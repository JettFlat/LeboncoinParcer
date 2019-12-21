using LeboncoinParcer;
using SQLiteAspNetCoreDemo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace LeboncoinParser
{
    public class VM : VMBase
    {
        string _Text = Parser.Timespan.ToString();
        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                Parser.Timespan = Int32.Parse(_Text);
                OnPropertyChanged();
            }
        }
        bool _Visible { get; set; } = true;
        public bool Visible
        {
            get => _Visible;
            set
            {
                _Visible = value;
                OnPropertyChanged();
            }
        }
        bool _UpVisible = true;
        public bool UpVisible
        {
            get => _UpVisible;
            set
            {
                _UpVisible = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand Start => new RelayCommand(o =>
        {
            Parser.IsRun = true;
            Subscribe();
             Visible = false;
            UpVisible = false;
            Task.Run(() => { Parser.Start();Visible = true; UpVisible = true; });

        });

        private void Parser_LogChanged()
        {
            Log = Parser.Log;
        }

        public RelayCommand Stop => new RelayCommand(o =>
        {
            Parser.IsRun = false;
        });
        public RelayCommand Export => new RelayCommand(o =>
        {
            //ADD your Export method here
        });
        public RelayCommand Update => new RelayCommand(o =>
        {
            Parser.IsRun = true;
            Subscribe();
            Visible = false;
            UpVisible = false;
            Task.Run(() => { Parser.UpdateDBitems(); Visible = true; UpVisible = true; });
            
        });
        public RelayCommand Clear => new RelayCommand(o =>
        {
            Parser.Log = "";
        });
        void Subscribe()
        {
            DataBase.DBUpdated += DataBase_DBUpdated;
            Parser.LogChanged += Parser_LogChanged;
        }
        private void DataBase_DBUpdated()
        {
            Realties= new ObservableCollection<Realty>(DataBase.Get());
        }
        string _Log=Parser.Log;
        public string Log
        {
            get => _Log;
            set
            {
                _Log = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Realty> _Realties =new ObservableCollection<Realty>(DataBase.Get());
        public ObservableCollection<Realty> Realties
        {
            get => _Realties;
            set
            {
                _Realties = value;
                OnPropertyChanged();
            }
        }
    }
}
