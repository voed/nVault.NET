using nVault.NET.ViewModel;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using nVault.NET.Helper;

namespace nVault.NET.Model
{
    public class EntryModel : ViewModelBase, IDataErrorInfo
    {
        private string _entryKey;
        private string _entryValue;
        private int _entryDateRaw;

        [StringLength(byte.MaxValue)]
        public string EntryKey
        {
            get => _entryKey;
            set
            {
                _entryKey = value;
                OnPropertyChanged(nameof(EntryKey));
            }
        }

        public string EntryValue
        {
            get => _entryValue;
            set
            {
                _entryValue = value;
                OnPropertyChanged(nameof(EntryValue));
            }
        }

        public DateTime EntryDate
        {
            get => Utils.UnixToDateTime(_entryDateRaw);
            set
            {
                _entryDateRaw = Utils.DateTimeToUnix(value);
                OnPropertyChanged(nameof(EntryDate));
                OnPropertyChanged(nameof(EntryDateRaw));
            }
        }

        public int EntryDateRaw
        {
            get => _entryDateRaw;
            set
            {
                _entryDateRaw = value;
                OnPropertyChanged(nameof(EntryDate));
                OnPropertyChanged(nameof(EntryDateRaw));
            }
        }

        [Display(AutoGenerateField = false)]
        public string Error => string.IsNullOrWhiteSpace(EntryKey) ? "Key cannot be empty" : string.Empty;

        public string this[string columnName] => columnName == nameof(EntryKey) && string.IsNullOrWhiteSpace(EntryKey) ? "Key cannot be empty" : string.Empty;
    }
}
