using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using HttpHeadersViewer.ViewModel.Base;

namespace HttpHeadersViewer.Helper
{
    internal static class NetworkHelper
    {
        public const string ONLINE = "Online";

        public const string OFFLINE = "Offline";

        private const int seconds = 1;

        private static HashSet<Action<string>> actions = new HashSet<Action<string>>();

        private static bool _status = false;

        public static bool Status => _status;

        public static string Label => Status ? ONLINE : OFFLINE; 
        
        private static void NetworkStatusUpdate(object sender, EventArgs e)
        {
            var currentStatus = InternetGetConnectedState(out var desc, 0);
            if (currentStatus != _status)
            {
                _status = currentStatus;
            }

            foreach (var action in actions)
            {
                action(Label);
            }
            UIHelper.Update(RelayCommand.RaiseCanExecuteChanged);
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static void StartNetworkTracking(Action<string> action)
        {
            var Timer = new DispatcherTimer();
            actions.Add(action);
            Timer.Tick += new EventHandler(NetworkStatusUpdate);
            Timer.Start();
            Timer.Interval = TimeSpan.FromSeconds(seconds);
        }
    }
}
