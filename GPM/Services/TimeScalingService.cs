using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Services
{
    public class TimeScalingService : ITimeScalingService
    {
        private double _timeScale = 1.0;

        public double GetTimeScale()
        {
            return _timeScale;
        }

        public void SetTimeScale(double scale)
        {
            _timeScale = scale;
        }
    }
}