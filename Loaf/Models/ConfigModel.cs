using System;
using System.Runtime.CompilerServices;

namespace Loaf.Models
{
    public class ConfigModel
    {
        private int _selectedIndex;
        private int _themeMode = 3;

        private double _time = 60;

        public int ThemeMode
        {
            get => _themeMode;
            set
            {
                _themeMode = value;
                OnPropertyChanged();
            }
        }

        public double Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public event Action PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke();
        }
    }
}