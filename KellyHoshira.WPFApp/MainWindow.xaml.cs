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

namespace KellyHoshira.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public KellyHoshiraBot Bot { get; set; }
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();

        public MainWindow()
        {
            // Create the Bot
            Bot = new KellyHoshiraBot();
            Bot.Client.MessageReceived += Client_MessageReceived;
            Bot.LogReceived += Bot_LogReceived;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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
                        Messages.Add(e.Message);
                    }
                })
            );

        }

        private void Bot_LogReceived(object sender, LogMessageEventArgs e)
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => Logs.Add(e.Message))
            );
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            await Bot.ConnectAsync();
            currentNetworkStatus.Text = Bot.NetworkStatus.ToString();
        }

        private async void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await Bot.DisconectAsync();
            currentNetworkStatus.Text = Bot.NetworkStatus.ToString();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Bot?.NetworkStatus == OnlineStatus.Online)
                await Bot.DisconectAsync();
        }
    }
}
