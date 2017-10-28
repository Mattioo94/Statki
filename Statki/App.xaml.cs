using System;
using System.Threading;
using System.Windows;

namespace Statki
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        private static MainWindow mainWindow = null;

        App()
        {
            InitializeComponent();
        }

        [STAThread]
        static void Main(string arg = "")
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                App app = new App();
                mainWindow = new MainWindow();
                app.Run(mainWindow);
                mutex.ReleaseMutex();
            }
            else
            {
                mainWindow.WindowState = WindowState.Normal;
            }
        }
    }
}
