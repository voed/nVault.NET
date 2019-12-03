using Microsoft.Win32;
using nVault.NET.Model;
using nVault.NET.ViewModel;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using nVault.NET.Helper;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using SelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace nVault.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Utils.SearchType _currSearchType = Utils.SearchType.SearchAll;
        private NVaultLib _nVault;
        private SearchExt _searchHelper;
        private VaultViewModel _model;
        private bool _dataLoaded;
        private bool _dataChanged;
        private string _currentFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _model = MainGrid.DataContext as VaultViewModel;
            _searchHelper = new SearchExt(MainGrid);
            _searchHelper.SetSearch(_currSearchType);
            MainGrid.SearchHelper = _searchHelper;
            MainGrid.CurrentCellValidating += MainGrid_CurrentCellValidating;

            var cmdline = Environment.GetCommandLineArgs();
            if(cmdline.Length == 2 && File.Exists(cmdline[1]))
                LoadVault(cmdline[1]);
#if DEBUG
            else
                LoadVault("example.vault");
#endif
        }

        private void MainGrid_CurrentCellValidating(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValidatingEventArgs e)
        {
            if (e.Column.MappingName == nameof(EntryModel.EntryKey))
            {
                if (_model.VaultCollection.Count(x => x.EntryKey.Equals(e.NewValue.ToString())) > 1)
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Key must be unique";
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(SearchTextBox.Text.Length > 2)
                MainGrid.SearchHelper.Search(SearchTextBox.Text);
            else
            {
                MainGrid.SearchHelper.ClearSearch();
            }
        }

        private void SearchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currSearchType = (Utils.SearchType)SearchComboBox.SelectedIndex;
            _searchHelper?.SetSearch(_currSearchType);

            if (MainGrid != null)
            {
                MainGrid.SearchHelper.ClearSearch();
                MainGrid.SearchHelper.Search(SearchTextBox.Text);
            }
        }

        private void SearchNext_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.SearchHelper.FindNext(SearchTextBox.Text);
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            var entry = new EntryModel
            {
                EntryKey = "",
                EntryValue = "",
                EntryDate = DateTime.Now,
                EntryDateRaw = 0
            };

            _model.VaultCollection.Add(entry);
            MainGrid.GetVisualContainer().ScrollOwner.ScrollToEnd();
            MainGrid.SelectedItem = entry;
            _dataChanged = true;
        }

        private void LoadVault(string filename)
        {
            _nVault = new NVaultLib(filename);
            _model.VaultCollection.Clear();
            foreach (EntryModel model in _nVault.Entries.Select(entry => new EntryModel
            {
                EntryKey = entry.Key,
                EntryValue = entry.Value,
                EntryDate = entry.Timestamp,
                EntryDateRaw = entry.TimestampRaw
            }))
            {
                _model?.VaultCollection.Add(model);
                model.PropertyChanged += Model_PropertyChanged;
            }

            _model.VaultVersion = _nVault.Version;
            MainGrid.GridColumnSizer.ResetAutoCalculationforAllColumns();
            MainGrid.GridColumnSizer.Refresh();
            _dataLoaded = true;
            _currentFile = filename;
            UpdateTitle();
        }

        private void SaveVault()
        {
            if (!_dataChanged)
                return;
            SaveVault(_currentFile);
            _dataChanged = false;
            UpdateTitle();
        }

        private void SaveVault(string filename)
        {
            NVaultLib vaultLib = new NVaultLib();
            foreach (EntryModel entryModel in _model.VaultCollection)
            {
                if (!string.IsNullOrWhiteSpace(entryModel.EntryKey))
                {
                    vaultLib.Entries.Add(new VaultEntry
                    {
                        Key = entryModel.EntryKey, 
                        Value = entryModel.EntryValue, 
                        TimestampRaw = entryModel.EntryDateRaw
                    });
                }
            }
            vaultLib.Version = _model.VaultVersion;
            vaultLib.Save(filename);
        }

        private void UpdateTitle()
        {
            if (_currentFile != null)
            {
                string change = _dataChanged ? "*" : string.Empty;
                Title = $"{Utils.ProgramName} - {Path.GetFileName(_currentFile)}{change}";
            }
            else
                Title = Utils.ProgramName;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_dataLoaded && !_dataChanged)
                OnDataChanged();
        }

        private void OnDataChanged()
        {
            _dataChanged = true;
            UpdateTitle();
        }

        #region Commands

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(Assembly.GetExecutingAssembly().Location);
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = Properties.Resources.VaultFileFilter, 
                DefaultExt = "vault"
            };

            if (dialog.ShowDialog(this) == true)
            {
                if (_dataLoaded)
                    Process.Start(Assembly.GetExecutingAssembly().Location, $"\"{dialog.FileName}\"");
                else
                    LoadVault(dialog.FileName);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveVault();
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = Properties.Resources.VaultFileFilter,
                DefaultExt = "vault"
            };
            if (dialog.ShowDialog(this) == true)
            {
                SaveVault(dialog.FileName);
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CloseApp())
            {
                Environment.Exit(0);
            }
        }

        private bool CloseApp()
        {
            if (_dataLoaded && _dataChanged)
            {
                var res = MessageBox.Show("Save changes?", 
                    "File was edited", 
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                    {
                        SaveVault();
                        return true;
                    }
                    case MessageBoxResult.No:
                    {
                        return true;
                    }
                    case MessageBoxResult.Cancel:
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CloseApp();
        }

        private void RemoveRowButton_Click(object sender, RoutedEventArgs e)
        {
            int index = MainGrid.SelectedIndex;

            EntryModel item = (EntryModel) MainGrid.SelectedItem;
            if (item == null)
                return;

            _model.VaultCollection.Remove(item);
            _dataChanged = true;
            if (MainGrid.GetRecordsCount() > 0)
            {
                MainGrid.SelectedItem = _model.VaultCollection[index - 1];
            }
        }
    }
}
