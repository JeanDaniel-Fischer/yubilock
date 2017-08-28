using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YubikeyLock
{
    public class YubikeyLockSystray : YubikeyUsbMonitorObserver
    {
        private NotifyIcon notifyIcon;
        private ContextMenu menu;
        private YubikeyUsbMonitor monitor;
        private MenuItem disabled;

        public YubikeyLockSystray(YubikeyUsbMonitor monitor)
        {
            System.ComponentModel.Container comp = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(comp);
            notifyIcon.Visible = true;
            menu = new ContextMenu();
            notifyIcon.ContextMenu = menu;
            this.monitor = monitor;
            this.disabled = new MenuItem("Disabled", disabledClick);
            menu.MenuItems.Add(disabled);
            menu.MenuItems.Add(new MenuItem("Exit", exitClick));
            this.statusChanged(!monitor.WatcherActivated, monitor.YubikeyPresent);
        }

        private void exitClick(object sender, EventArgs e)
        {
            this.monitor.stopMonitoring();
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void disabledClick(object sender, EventArgs e)
        {
            if (this.monitor.WatcherActivated)
            {
                disabled.Checked = true;
                this.monitor.stopMonitoring();
            }
            else
            {
                this.monitor.startMonitoring();
                disabled.Checked = false;
            }
        }

        public void statusChanged(bool disabled, bool available)
        {
            if (disabled)
            {
                notifyIcon.Icon = YubikeyLock.Properties.Resources.Disabled;
                notifyIcon.Text = "Yubikey locker disabled";
            }
            else
            {
                if (available)
                {
                    notifyIcon.Icon = YubikeyLock.Properties.Resources.Available;
                    notifyIcon.Text = "Yubikey locker, key detected";
                }
                else
                {
                    notifyIcon.Icon = YubikeyLock.Properties.Resources.NotAvailable;
                    notifyIcon.Text = "Yubikey locker, key missing";
                }
            }
        }
    }
}
