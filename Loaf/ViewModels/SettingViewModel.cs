using System.Reflection;
using Loaf.Models;
using Prism.Mvvm;

namespace Loaf.ViewModels
{
    public class SettingViewModel : BindableBase
    {
        private readonly ConfigModel _model;
        private int _selectedIndex;
        private int _themeMode;

        private double _time;

        public SettingViewModel(ConfigModel model)
        {
            _model = model;
            ThemeMode = _model.ThemeMode;
            Time = _model.Time;
            SelectedIndex = _model.SelectedIndex;
        }

        public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";

        public int ThemeMode
        {
            get => _themeMode;
            set
            {
                SetProperty(ref _themeMode, value);
                _model.ThemeMode = value;
            }
        }

        public double Time
        {
            get => _time;
            set
            {
                SetProperty(ref _time, value);
                _model.Time = value;
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                SetProperty(ref _selectedIndex, value);
                _model.SelectedIndex = value;
            }
        }
    }
}