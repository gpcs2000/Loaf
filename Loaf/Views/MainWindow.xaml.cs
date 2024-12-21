using System;
using System.Windows;
using Loaf.Event;
using Microsoft.Win32;
using Prism.Events;

namespace Loaf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEventAggregator _aggregator;

        public MainWindow(IEventAggregator aggregator)
        {
            InitializeComponent();
            _aggregator = aggregator;

            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                _aggregator.GetEvent<ChangeThemeEvent>().Publish();
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            base.OnClosed(e);
        }
    }
}