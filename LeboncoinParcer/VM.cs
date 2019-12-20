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
        public RelayCommand Start => new RelayCommand(o =>
        {
            Visible = false;
            Task.Run(() => Parser.Start());
        });
        ObservableCollection<Realty> _Realties =new ObservableCollection<Realty>(SQLiteDBContext.Get(20));
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
