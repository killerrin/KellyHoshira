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

        public TaskbarIcon TaskBarIcon;

        public MainWindow()
        {
            // Create the TaskBarIcon
            TaskBarIcon = new TaskbarIcon();
            TaskBarIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Icons/Offline.ico"));
            TaskBarIcon.ToolTipText = "Kelly Hoshira - Discord Bot";
            TaskBarIcon.TrayMouseDoubleClick += TaskBarIcon_TrayMouseDoubleClick;
            TaskBarIcon.Visibility = Visibility.Collapsed;

            // Create the Bot
            Bot = new KellyHoshiraBot();
            Bot.Client.MessageReceived += Client_MessageReceived;
            Bot.LogReceived += Bot_LogReceived;
            Bot.NetworkChanged += Bot_NetworkChanged;

            // Initialize the Program
            InitializeComponent();
        }

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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

        private void TaskBarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            TaskBarIcon.Visibility = Visibility.Collapsed;
            Show();
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

        private void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    if (e.Message.IsMentioningMe(true))
                    {
                        Messages.Add($"{DateTime.Now} - {e.Message}");
                    }
                })
            );
        }

        private void Bot_LogReceived(object sender, LogMessageEventArgs e)
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
