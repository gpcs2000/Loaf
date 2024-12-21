using System.Globalization;
using System.Threading;
using System.Windows;
using Loaf.Config;
using Loaf.Models;
using Loaf.ViewModels;
using Loaf.Views;
using Microsoft.Win32;
using Prism.Ioc;
using Prism.Navigation.Regions;

namespace Loaf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly ConfigModel _model;

        public App()
        {
            var config = new JsonConfigManager();
            _model = config.ConfigModel;
            LanguageManager.Instance.ChangeLanguage(Thread.CurrentThread.CurrentUICulture);
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category != UserPreferenceCategory.Locale)
                return;

            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            switch (currentCulture.Name.ToUpper())
            {
                case "EN-US":
                case "ZH-TW":
                case "ZH-CN":
                    LanguageManager.Instance.ChangeLanguage(currentCulture);
                    break;
                default:
                    LanguageManager.Instance.ChangeLanguage(new CultureInfo("en-US"));
                    break;
            }
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainView));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.Register<FullScreenWindow>();
            containerRegistry.Register<FullScreenWindowViewModel>();
            containerRegistry.RegisterInstance(_model);
        }
    }
}