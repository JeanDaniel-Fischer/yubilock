using System;
using System.Windows.Forms;
using System.Management;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Threading;
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
        private static string appGuid = "E38E3950-979E-4F4A-96B2-67B75F769DDC";

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, @"Global\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Instance already running");
                    return;
                }

                GC.Collect();
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
}
