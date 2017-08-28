using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YubikeyLock
{
    public interface YubikeyUsbMonitorObserver
    {
        void statusChanged(bool disabled, bool available);
    }
}
