using Discord;
using KellyHoshira.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using Discord.WebSocket;

namespace KellyHoshira.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public KellyHoshiraBot Bot { get; set; }
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();

        public TaskbarIcon TaskBarIcon { get { return taskBarIcon; } }

        public MainWindow()
        {
            // Load the Keys
            SecretKeys keys = SecretKeys.Load("Secret.txt");

            // Create the Bot
            Bot = new KellyHoshiraBot(keys);
            Bot.OnMessageReceived += Bot_OnMessageReceived;
            Bot.OnLogReceived += Bot_OnLogReceived;
            Bot.NetworkChanged += Bot_NetworkChanged;

            // Initialize the Program
            InitializeComponent();
        }

        public void ReviveFromTray()
        {
            showButton_Click(this, new RoutedEventArgs());
        }
        public void MinimizeToTray()
        {
            minimizeToTrayButton_Click(this, new RoutedEventArgs());
        }

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.StartOnline)
                connectButton_Click(this, null);
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    break;
                case WindowState.Minimized:
                    minimizeToTrayButton_Click(sender, new RoutedEventArgs());
                    break;
                case WindowState.Normal:
                    break;
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Bot?.NetworkStatus == OnlineStatus.Online)
                await Bot.DisconectAsync();
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            await Bot.ConnectAsync();
        }

        private async void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await Bot.DisconectAsync();
        }

        private void showButton_Click(object sender, RoutedEventArgs e)
        {
            TaskBarIcon.Visibility = Visibility.Collapsed;
            Show();
            WindowState = WindowState.Normal;
        }

        private void minimizeToTrayButton_Click(object sender, RoutedEventArgs e)
        {
            TaskBarIcon.Visibility = Visibility.Visible;
            Hide();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region Kelly Events
        private void Bot_NetworkChanged(object sender, Core.Events.NetworkChangedEventArgs args)
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    currentNetworkStatus.Text = Bot.NetworkStatus.ToString();

                    switch (Bot.NetworkStatus)
                    {
                        case OnlineStatus.Online:
                            currentNetworkStatusIndicator.Fill = Brushes.Green;
                            TaskBarIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Icons/Online.ico"));
                            break;
                        case OnlineStatus.Offline:
                            currentNetworkStatusIndicator.Fill = Brushes.Red;
                            TaskBarIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Icons/Offline.ico"));
                            break;
                        case OnlineStatus.Unavailable:
                            currentNetworkStatusIndicator.Fill = Brushes.Yellow;
                            TaskBarIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Icons/Unavailable.ico"));
                            break;
                    }
                })
            );
        }

        private void Bot_OnMessageReceived(object sender, SocketUserMessage e)
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    Messages.Add($"{DateTime.Now} - {e.Content}");
                })
            );
        }

        private void Bot_OnLogReceived(object sender, LogMessage e)
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => Logs.Add($"{DateTime.Now} - {e.Message}"))
            );
        }
        #endregion

        #endregion
    }
}
