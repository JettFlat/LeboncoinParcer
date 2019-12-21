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

        #region Filters
        //string _NameFilter="";
        //public string NameFilter
        //{
        //    get => _NameFilter;
        //    set
        //  {
        //        _NameFilter = value;
        //        if(!string.IsNullOrWhiteSpace(_NameFilter))
        //        {
        //            Realties = new ObservableCollection<Realty>( Realties.Where(x => x.Name.Contains(_NameFilter)).ToList());
        //            var MyView = CollectionViewSource.GetDefaultView(Realties);
        //            MyView.
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        //string _PhoneFilter = "";
        //public string PhoneFilter
        //{
        //    get => _PhoneFilter;
        //    set
        //    {
        //        _PhoneFilter = value;
        //        if (!string.IsNullOrWhiteSpace(_PhoneFilter))
        //        {
        //            Realties = new ObservableCollection<Realty>(Realties.Where(x => x.Name.Contains(_PhoneFilter)).ToList());
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        //string _LocationFilter = "";
        //public string LocationFilter
        //{
        //    get => _LocationFilter;
        //    set
        //    {
        //        _LocationFilter = value;
        //        if (!string.IsNullOrWhiteSpace(_LocationFilter))
        //        {
        //            Realties = new ObservableCollection<Realty>(Realties.Where(x => x.Name.Contains(_LocationFilter)).ToList());
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        //string _TypeFilter = "";
        //public string TypeFilter
        //{
        //    get => _TypeFilter;
        //    set
        //    {
        //        _TypeFilter = value;
        //        if (!string.IsNullOrWhiteSpace(_TypeFilter))
        //        {
        //            Realties = new ObservableCollection<Realty>(Realties.Where(x => x.Name.Contains(_TypeFilter)).ToList());
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        //string _SurfaceFilter = "";
        //public string SurfaceFilter
        //{
        //    get => _SurfaceFilter;
        //    set
        //    {
        //        _SurfaceFilter = value;
        //        if (!string.IsNullOrWhiteSpace(_SurfaceFilter))
        //        {
        //            Realties = new ObservableCollection<Realty>(Realties.Where(x => x.Name.Contains(_SurfaceFilter)).ToList());
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        //string _FurnitureFilter = "";
        //public string FurnitureFilter
        //{
        //    get => _FurnitureFilter;
        //    set
        //    {
        //        _FurnitureFilter = value;
        //        if (!string.IsNullOrWhiteSpace(_FurnitureFilter))
        //        {
        //            Realties = new ObservableCollection<Realty>(Realties.Where(x => x.Name.Contains(_FurnitureFilter)).ToList());
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        //string _GesFilter = "";
        //public string GesFilter
        //{
        //    get => _GesFilter;
        //    set
        //    {
        //        _GesFilter = value;
        //        if (!string.IsNullOrWhiteSpace(_GesFilter))
        //        {
        //            Realties = new ObservableCollection<Realty>(Realties.Where(x => x.Name.Contains(_GesFilter)).ToList());
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        //string _EnergyClass = "";
        //public string EnergyClass
        //{
        //    get => _EnergyClass;
        //    set
        //    {
        //        _EnergyClass = value;
        //        if (!string.IsNullOrWhiteSpace(_EnergyClass))
        //        {
        //            Realties = new ObservableCollection<Realty>(Realties.Where(x => x.Name.Contains(_EnergyClass)).ToList());
        //        }
        //        OnPropertyChanged();
        //    }
        //}

        #endregion

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
            Parser.Export();
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
