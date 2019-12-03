using nVault.NET.Model;
using System;
using System.Collections.ObjectModel;

namespace nVault.NET.ViewModel
{
    public class VaultViewModel : ViewModelBase, IDisposable
    {
        private ushort _vaultVersion;
        private ObservableCollection<EntryModel> _vaultCollection;

        public ObservableCollection<EntryModel> VaultCollection
        {
            get => _vaultCollection;
            set
            {
                _vaultCollection = value;

                OnPropertyChanged(nameof(VaultCollection));
            }
        }

        public ushort VaultVersion
        {
            get => _vaultVersion;
            set
            {
                _vaultVersion = value;
                OnPropertyChanged(nameof(VaultVersion));
            }
        }

        public VaultViewModel()
        {
            _vaultCollection = new ObservableCollection<EntryModel>();
        }

        public void Dispose()
        {
            VaultCollection?.Clear();
        }
    }

}
