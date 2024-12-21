using System;
using System.Timers;
using Loaf.Event;
using Loaf.Models;
using Prism.Events;
using Prism.Mvvm;

namespace Loaf.ViewModels
{
    public class FullScreenWindowViewModel : BindableBase
    {
        private readonly IEventAggregator _aggregator;
        private readonly double _planedTime;
        private readonly DateTime _startTime;
        private readonly Timer _timer;
        private int _progress;

        public FullScreenWindowViewModel(ConfigModel model, IEventAggregator aggregator)
        {
            _timer = new Timer(1000);
            _startTime = DateTime.Now;
            _planedTime = model.Time * Math.Pow(60, model.SelectedIndex);
            _aggregator = aggregator;
            _timer.Elapsed += ChangeProgress;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
        }

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private void ChangeProgress(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _startTime).TotalSeconds >= _planedTime)
            {
                Progress = 100;
                _timer.Stop();
                _aggregator.GetEvent<CloseEvent>().Publish();
                return;
            }

            Progress = (int) ((DateTime.Now - _startTime).TotalSeconds / _planedTime * 100);
        }
    }
}