using System.Linq;
using iNKORE.UI.WPF.Modern;
using Loaf.Event;
using Loaf.Models;
using Loaf.Utils;
using Loaf.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace Loaf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator _aggregator;
        private readonly ConfigModel _model;
        private readonly IRegionManager _regionManager;

        private bool _isVisible;
        private ElementTheme _theme;


        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator aggregator, ConfigModel model)
        {
            _regionManager = regionManager;
            _aggregator = aggregator;
            _model = model;
            NavigateCommand = new DelegateCommand(NavigateToMainView);

            _aggregator.GetEvent<ChangeViewEvent>().Subscribe(ChangeButtonStatus);
            _aggregator.GetEvent<ChangeThemeEvent>().Subscribe(ChangeTheme);
            _model.PropertyChanged += ChangeTheme;
            ChangeTheme();
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }


        public DelegateCommand NavigateCommand { get; }

        public ElementTheme Theme
        {
            get => _theme;
            set => SetProperty(ref _theme, value);
        }

        private void ChangeTheme()
        {
            Theme = _model.ThemeMode switch
            {
                1 => ElementTheme.Light,
                2 => ElementTheme.Dark,
                3 => ThemeHelper.IsDarkTheme switch
                {
                    true => ElementTheme.Dark,
                    false => ElementTheme.Light,
                    _ => ElementTheme.Default
                },
                _ => Theme
            };
        }

        private void ChangeButtonStatus(bool obj)
        {
            IsVisible = obj;
        }

        private void NavigateToMainView()
        {
            var currentView = _regionManager.Regions["ContentRegion"].ActiveViews.FirstOrDefault();
            if (currentView is not SettingView)
                return;

            _regionManager.RequestNavigate("ContentRegion", "MainView");
            _aggregator.GetEvent<ChangeViewEvent>().Publish(false);
        }
    }
}