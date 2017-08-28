using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace YubikeyLock
{
    public class YubikeyUsbMonitor
    {
        private bool yubikeyAvailable;
        private ManagementEventWatcher watcher;
        private bool watchActivated;
        private List<YubikeyUsbMonitorObserver> observer = new List<YubikeyUsbMonitorObserver>(2);

        public bool WatcherActivated
        {
            get
            {
                return this.watchActivated;
            }
        }
        public bool YubikeyPresent
        {
            get
            {
                return this.yubikeyAvailable;
            }
        }

        public YubikeyUsbMonitor()
        {
            this.watcher = new ManagementEventWatcher();
            // Configure watcher
            WqlEventQuery query = new WqlEventQuery(
                "SELECT * FROM Win32_DeviceChangeEvent");
            this.watcher.EventArrived += new EventArrivedEventHandler(eventArrived);
            this.watcher.Query = query;
            this.watchActivated = false;
            this.yubikeyAvailable = this.isYubikeyConnected();
        }

        public void addObserver(YubikeyUsbMonitorObserver observer)
        {
            this.observer.Add(observer);
        }

        public void startMonitoring()
        {
            if (!this.watchActivated)
            {
                this.watchActivated = true;
                this.eventArrived(null, null); // Refresh status of available
                foreach (YubikeyUsbMonitorObserver obs in this.observer)
                {
                    obs.statusChanged(!this.WatcherActivated, this.YubikeyPresent);
                }
                this.watcher.Start();
            }
        }

        public void stopMonitoring()
        {
            if (this.watchActivated)
            {
                this.watchActivated = false;
                this.watcher.Stop();
                foreach (YubikeyUsbMonitorObserver obs in this.observer)
                {
                    obs.statusChanged(!this.WatcherActivated, this.YubikeyPresent);
                }
            }
        }

        // Request device list for usb device and filter on Yubikey Vendor ID
        private bool isYubikeyConnected()
        {
            bool found = false;
            ObjectQuery q = new ObjectQuery("Select * FROM Win32_USBHub");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(q);
            ManagementObjectCollection coll = mos.Get();
            ManagementObjectCollection.ManagementObjectEnumerator usbObj = coll.GetEnumerator();
            while (!found && usbObj.MoveNext())
            {
                ManagementBaseObject mbo = usbObj.Current;
                found = found || (mbo.Properties["PNPDeviceID"].Value.ToString().StartsWith("USB\\VID_1050"));
            }
            return found;
        }

        private void eventArrived(object sender, EventArrivedEventArgs e)
        {
            if (watchActivated)
            {
                bool available = this.isYubikeyConnected();
                if (this.yubikeyAvailable != available)
                {
                    Console.Out.WriteLine("Status changed to {0}", available);
                    this.yubikeyAvailable = available;
                    foreach (YubikeyUsbMonitorObserver obs in this.observer)
                    {
                        obs.statusChanged(!this.WatcherActivated, this.YubikeyPresent);
                    }
                }
            }
        }
    }
}
