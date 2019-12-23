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
        public VM() : base()
        {
            Subscribe();
            MovieView = GetMovieCollectionView(Realties);
            MovieView.Filter = OnFilterMovie;
        }
        string _Text = Parser.ProxyTimeout.ToString();
        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                Parser.ProxyTimeout = Int32.Parse(_Text);
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

        bool _ExportEnable = true;
        public bool ExportEnable
        {
            get => _ExportEnable;
            set
            {
                _ExportEnable = value;
                OnPropertyChanged();
            }
        }
        //ExportEnable
        #region Filters
        public void UpdateDataGrid()
        {
            MovieView.Refresh();
        }
        string _PhoneFilter = "";
        public string PhoneFilter
        {
            get => _PhoneFilter;
            set
            {
                _PhoneFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _NameFilter = "";
        public string NameFilter
        {
            get => _NameFilter;
            set
            {
                _NameFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _LocationFilter = "";
        public string LocationFilter
        {
            get => _LocationFilter;
            set
            {
                _LocationFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _TypeFilter = "";
        public string TypeFilter
        {
            get => _TypeFilter;
            set
            {
                _TypeFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _SurfaceFilter = "";
        public string SurfaceFilter
        {
            get => _SurfaceFilter;
            set
            {
                _SurfaceFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _FurnitureFilter = "";
        public string FurnitureFilter
        {
            get => _FurnitureFilter;
            set
            {
                _FurnitureFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _GesFilter = "";
        public string GesFilter
        {
            get => _GesFilter;
            set
            {
                _GesFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _EnergyClassFilter = "";
        public string EnergyClassFilter
        {
            get => _EnergyClassFilter;
            set
            {
                _EnergyClassFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _StatusFilter = "";
        public string StatusFilter
        {
            get => _StatusFilter;
            set
            {
                _StatusFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _RoomsFilter = "";
        public string RoomsFilter
        {
            get => _RoomsFilter;
            set
            {
                _RoomsFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _DateFilter = "";
        public string DateFilter
        {
            get => _DateFilter;
            set
            {
                _DateFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        string _UrlFilter = "";
        public string UrlFilter
        {
            get => _UrlFilter;
            set
            {
                _UrlFilter = value;
                OnPropertyChanged();
                UpdateDataGrid();
            }
        }
        CollectionView _MovieView;
        public CollectionView MovieView
        {
            get => _MovieView;
            set
            {
                _MovieView = value;
                OnPropertyChanged();
            }
        }
        public CollectionView GetMovieCollectionView(ObservableCollection<Realty> movList)
        {
            return (CollectionView)CollectionViewSource.GetDefaultView(movList);
        }
        bool OnFilterMovie(object item)
        {
            //TODO
            Realty t = item as Realty;
            if (t != null)
            {
                if (t.Phone.Contains(PhoneFilter) && t.Name.Contains(NameFilter) && t.LocalisationTown.Contains(LocationFilter) &&
                    t.Type.Contains(TypeFilter) && t.Surface.Contains(SurfaceFilter) && t.Furniture.Contains(FurnitureFilter) &&
                    t.Ges.Contains(GesFilter) && t.EnergyClass.Contains(EnergyClassFilter) && t.Status.Contains(StatusFilter) &&
                    t.Rooms.ToString().Contains(RoomsFilter) && t.Date.ToString("MM/dd/yyyy h:mm tt").Contains(DateFilter) &&
                    t.Url.Contains(UrlFilter))
                    return true;
            }
            return false;
        }
        #endregion

        public RelayCommand Start => new RelayCommand(o =>
        {
            try
            {
                Parser.Resettoken();
                Visible = false;
                UpVisible = false;
                Task.Run(() => { Parser.Start(); Visible = true; UpVisible = true; });
            }
            catch (Exception exc)
            {
                exc.Write(MainWindow.Locker);
            }

        });

        void Parser_LogChanged()
        {
            Log = Parser.Log;
        }

        public RelayCommand Stop => new RelayCommand(o =>
        {
            try
            {
                Parser.Stoptoken();
            }
            catch (Exception exc)
            {
                exc.Write(MainWindow.Locker);
            }

        });
        public RelayCommand Export => new RelayCommand(o =>
        {
            try
            {
                ExportEnable = false;
                Task.Run(() => Parser.Export()); ExportEnable = true; ;
            }
            catch (Exception exc)
            {
                exc.Write(MainWindow.Locker);
            }
        });
        public RelayCommand Update => new RelayCommand(o =>
        {
            try
            {
                Parser.Resettoken();
                Visible = false;
                UpVisible = false;
                Task.Run(() => { Parser.UpdateDBitems(Parser.Token); Visible = true; UpVisible = true; });
            }
            catch (Exception exc)
            {
                exc.Write(MainWindow.Locker);
            }
        });
        public RelayCommand Clear => new RelayCommand(o =>
        {
            try
            {
                Parser.Log = "";
            }
            catch (Exception exc)
            {
                exc.Write(MainWindow.Locker);
            }
        });
        public RelayCommand Reqnavigate => new RelayCommand(o =>
        {
            try
            {
                var ps = new ProcessStartInfo((o as Realty).Url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
            catch (Exception exc)
            {
                exc.Write(MainWindow.Locker);
            }
        });
        void Subscribe()
        {
            DataBase.DBUpdated += DataBase_DBUpdated;
            Parser.LogChanged += Parser_LogChanged;
        }
        private void DataBase_DBUpdated()
        {
            Realties = new ObservableCollection<Realty>(DataBase.Get());
            MovieView = GetMovieCollectionView(Realties);
            MovieView.Filter = OnFilterMovie;
        }
        string _Log = Parser.Log;
        public string Log
        {
            get => _Log;
            set
            {
                _Log = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Realty> _Realties = new ObservableCollection<Realty>(DataBase.Get());
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
