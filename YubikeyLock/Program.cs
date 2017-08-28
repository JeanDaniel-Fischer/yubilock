using System;
using System.Windows.Forms;
using System.Management;
using System.Windows.Interop;
using System.Runtime.InteropServices;
// TODO
// Register listen device
// Close Register properly
// systray icon green if yubikey
// systray red if no yubikey
// init find if yubikey present
// lock on remove if log
// Remove form
namespace YubikeyLock
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationContext context = new ApplicationContext();

            YubikeyUsbMonitor monitor = new YubikeyUsbMonitor();
            YubikeyLockSystray systray = new YubikeyLockSystray(monitor);
            YubikeyScreenLocker locker = new YubikeyScreenLocker(monitor);
            monitor.addObserver(systray);
            monitor.addObserver(locker);
            monitor.startMonitoring();
            Application.Run(context);
        }
    }
}
