using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
        private const string DatabaseFileName = "Leboncoin.db";
        private const string LogFile = "Log.txt";
        private const string BackupExt = "bak";
        private readonly Random _random = new Random(DateTime.UtcNow.Millisecond);
        private readonly IDictionary<string, string> filters = new Dictionary<string, string>();
        private readonly object _locker = new object();

        public MainWindow()
        {
            //InitializeComponent();
            //InitDataGrid();
            //LeboncoinParcer.Test.Testing();
            LeboncoinParcer.Parser.Start();
        }

        private void InitDataGrid()
        {
            if(!File.Exists(DatabaseFileName))
                return;
            LeboncoinDataGrid.ItemsSource = SelectAdverts();
        }

        private AdvertModel[] SelectAdverts()
        {
            throw new NotImplementedException();
        }
        
        private void InsertAdvert(AdvertModel advert)
        {
            throw new NotImplementedException();
        }

        private void CreateTable()
        {
            throw new NotImplementedException();
        }

        private async Task ProcessPage(string url)
        {
            throw new NotImplementedException();
        }
        
        private async Task<int> GetPagesCount()
        {
            throw new NotImplementedException();
        }

        private void ExportButtonOnClick(object sender, RoutedEventArgs e)
        {
            //TODO Export visible rows from grid to google
            throw new NotImplementedException();
        }
        
        private void StopParseButtonOnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private async void NewParseButtonOnClick(object sender, RoutedEventArgs e)
        {
            await RunParsing(false);
        }
        
        private async void ContinueParseButtonOnClick(object sender, RoutedEventArgs e)
        {
            await RunParsing(true);
        }
        
        
        private void LeboncoinDataGridOnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var stackFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            stackFactory.SetValue(StackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            
            var labelFactory = new FrameworkElementFactory(typeof(Label));
            labelFactory.SetValue(Label.ContentProperty, e.PropertyName);
            labelFactory.SetValue(Label.FlowDirectionProperty, FlowDirection.RightToLeft);
            var textboxFactory = new FrameworkElementFactory(typeof(TextBox));
            textboxFactory.SetValue(TextBox.NameProperty, e.PropertyName);
            textboxFactory.SetValue(TextBox.WidthProperty, 200.0);
            textboxFactory.SetValue(TextBox.FlowDirectionProperty, FlowDirection.RightToLeft);
            textboxFactory.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(FilterTextChanged));
            
            stackFactory.AppendChild(labelFactory);
            stackFactory.AppendChild(textboxFactory);
            e.Column.HeaderTemplate = new DataTemplate(e.PropertyType) {VisualTree = stackFactory};
        }
        
        private void FilterTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var filter = textBox.Text;
            var collectionView = CollectionViewSource.GetDefaultView(LeboncoinDataGrid.ItemsSource);
            filters[textBox.Name] = filter;
            collectionView.Filter = o =>
            {
                var model = o as AdvertModel;
                
                var results = new List<bool>();

                foreach (var f in filters)
                {
                    if (string.IsNullOrEmpty(f.Value))
                    {
                        results.Add(true);
                        continue;
                    }
                    var value = (string) model.GetType().GetProperty(f.Key).GetValue(model, null);
                    results.Add(value.Contains(f.Value, StringComparison.OrdinalIgnoreCase));
                }

                return results.All(r => r == true);
            };
        }
        
        private void CreateDatabase()
        {
            if (File.Exists(DatabaseFileName))
                return;

            SQLiteConnection.CreateFile(DatabaseFileName);
            CreateTable();
        }
        
        private async Task RunParsing(bool continueParsing = false)
        {
            StatusLabel.Content = "Processing...";
            var pagesCount = await GetPagesCount();
            var startPage = Convert.ToInt32(StartPageTextBox.Text);

            if (startPage < 1 || startPage > pagesCount)
            {
                MessageBox.Show($"Start page can't be less than 1 or greater than {pagesCount} - total pages count");
                return;
            }
            
            LeboncoinDataGrid.ItemsSource = new List<AdvertModel>();

            if (!continueParsing)
            {
                StatusLabel.Content = "Creating backup...";
                var datetime = DateTime.Now.ToString("ddMMyyyyhhmmss");
                if (File.Exists(DatabaseFileName))
                    File.Move(DatabaseFileName, $"{DatabaseFileName}.{BackupExt}-{datetime}");
                CreateDatabase();
            }

            //TODO Parsing; Use StatusLabel to show current progress; update grid in progress; use sqllite to store data + better to use orm, but ado okay too
            
            InitDataGrid();
            StatusLabel.Content = "Done.";
        }
        
        private void Log(string message)
        {
            try
            {
                lock (_locker)
                {
                    File.AppendAllLines(LogFile, new[] { $"[{DateTime.Now}] {message}" });
                }
            }
            catch (Exception)
            {
            }
        }
        
        private async Task Reprocessor(Func<Task> action, string url)
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    await action();
                    break;
                }
                catch (Exception e)
                {
                    if (i == 9)
                    {
                        Log($"Fatal error. Max attempts counts reached. Url: {url}. Message: {e.Message}. StackTrace: {e.StackTrace}. Skipping: {SkipFatalErrorsCheckBox.IsChecked}");
                        if (SkipFatalErrorsCheckBox.IsChecked == null || !SkipFatalErrorsCheckBox.IsChecked.Value)
                        {
                            MessageBox.Show("Fatal error. Exiting. Please check Log.txt");
                            throw;
                        }
                        else 
                            break;
                    }
                    Log($"Error. Trying to reporcess after sleep. Attempt {i}.  Url: {url}. Message: {e.Message}");

                    if (i > 3)
                        Thread.Sleep(_random.Next(30000, 60000));
                    else
                        Thread.Sleep(_random.Next(2000, 3000));
                }
            }
        }
    }
}