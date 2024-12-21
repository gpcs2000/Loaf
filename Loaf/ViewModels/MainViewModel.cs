using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Loaf.Event;
using Loaf.Models;
using Loaf.Utils;
using Loaf.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace Loaf.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IEventAggregator _aggregator;
        private readonly IContainerExtension _container;
        private readonly ConfigModel _model;
        private readonly IRegionManager _regionManager;

        private ImageSource _source;

        public MainViewModel(IRegionManager regionManager, IEventAggregator aggregator, ConfigModel model, IContainerExtension container)
        {
            _regionManager = regionManager;
            _aggregator = aggregator;
            _model = model;
            _container = container;
            NavigateCommand = new DelegateCommand(NavigateToSettingView);
            StartCommand = new DelegateCommand(OpenFullScreen);
            aggregator.GetEvent<ChangeThemeEvent>().Subscribe(ChangeImage);
            _model.PropertyChanged += ChangeImage;
            ChangeImage();
        }


        public DelegateCommand NavigateCommand { get; }
        public DelegateCommand StartCommand { get; }

        public ImageSource Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        private void ChangeImage()
        {
            Source = _model.ThemeMode switch
            {
                1 => new BitmapImage(new Uri("pack://application:,,,/Assets/relaxing_light.png")),
                2 => new BitmapImage(new Uri("pack://application:,,,/Assets/relaxing_dark.png")),
                3 => ThemeHelper.IsDarkTheme switch
                {
                    true => new BitmapImage(new Uri("pack://application:,,,/Assets/relaxing_dark.png")),
                    _ => new BitmapImage(new Uri("pack://application:,,,/Assets/relaxing_light.png"))
                },
                _ => Source
            };
        }

        private void OpenFullScreen()
        {
            FullScreenWindow window = _container.Resolve<FullScreenWindow>();
            window.Show();
        }

        private void NavigateToSettingView()
        {
            _regionManager.RequestNavigate("ContentRegion", nameof(SettingView));
            _aggregator.GetEvent<ChangeViewEvent>().Publish(true);
        }
    }
}