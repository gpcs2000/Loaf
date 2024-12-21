using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Loaf.Config
{
    public class LanguageManager : INotifyPropertyChanged
    {
        private static readonly Lazy<LanguageManager> _lazy = new();
        private readonly ResourceManager _resourceManager;

        public LanguageManager()
        {
            _resourceManager = new ResourceManager("Loaf.Resources.Lang", typeof(LanguageManager).Assembly);
        }

        public string this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                return _resourceManager.GetString(name);
            }
        }

        public static LanguageManager Instance => _lazy.Value;
        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeLanguage(CultureInfo cultureInfo)
        {
            CultureInfo.CurrentUICulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item[]"));
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}