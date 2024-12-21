using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Prism.Navigation.Regions;

namespace Loaf.Views
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView : UserControl, IConfirmNavigationRequest
    {
        private readonly Storyboard _enterAnimation;
        private readonly Storyboard _exitAnimation;


        public SettingView()
        {
            InitializeComponent();
            // 获取动画引用
            _enterAnimation = (Storyboard) FindResource("EnterAnimation");
            _exitAnimation = (Storyboard) FindResource("ExitAnimation");

            // 注册Loaded事件
            Loaded += UserControl_Loaded;
        }


        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            _exitAnimation.Completed += (_, _) =>
            {
                continuationCallback(true);
            };
            _exitAnimation?.Begin(this);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }


        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _enterAnimation?.Begin(this);
        }
    }
}