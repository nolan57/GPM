using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPM.Services;

namespace GPM.ViewModels
{
    public class TimeScalingViewModel : BindableBase
    {
        private readonly ITimeScalingService _timeScalingService;

        private double _timeScale = 1.0;
        public double TimeScale
        {
            get { return _timeScale; }
            set 
            { 
                SetProperty(ref _timeScale, value);
                _timeScalingService.SetTimeScale(value);
            }
        }

        public TimeScalingViewModel(ITimeScalingService timeScalingService)
        {
            _timeScalingService = timeScalingService;
            _timeScale = _timeScalingService.GetTimeScale();
        }
    }
}