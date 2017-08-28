using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YubikeyLock
{
    public class YubikeyScreenLocker : YubikeyUsbMonitorObserver
    {
        private bool previousState;

        public YubikeyScreenLocker(YubikeyUsbMonitor monitor)
        {
            this.previousState = monitor.YubikeyPresent;
        }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        public void statusChanged(bool disabled, bool available)
        {
            if (this.previousState && !available && !disabled)
            {
                this.previousState = false;
                YubikeyScreenLocker.LockWorkStation();
                Console.Out.WriteLine("Screen lock");
            }
            else if (disabled)
            {
                this.previousState = false;
            }
            else
            {
                this.previousState = available;
            }
        }
    }
}
