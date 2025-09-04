using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPM.Services
{
    public interface ITimeScalingService
    {
        double GetTimeScale();
        void SetTimeScale(double scale);
    }
}