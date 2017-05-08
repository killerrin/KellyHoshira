using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace KellyHoshira.WPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool StartMinimized = false;
        public static bool StartOnline = false;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/StartMinimized")
                {
                    StartMinimized = true;
                }
                if (e.Args[i] == "/StartOnline")
                {
                    StartOnline = true;
                }
            }

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            if (StartMinimized)
                mainWindow.WindowState = WindowState.Minimized;
            mainWindow.Show();

            if (StartMinimized)
                mainWindow.MinimizeToTray();
        }
    }
}
