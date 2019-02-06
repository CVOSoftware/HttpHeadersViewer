using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace HttpHeadersViewer.Common
{
    class StatusNetwork
    {
        #region Fields
        private bool status;                     // Connection status
        private byte seconds;                    // Connection status check interval
        private Border border;                   // Link to NetworkStatusIcon
        private TextBlock textBlock;             // link to TextBlockStatus
        #endregion

        #region Properties
        public bool Status => status;            // Connection status
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public StatusNetwork(Border border, TextBlock textBlock)
        {
            status = false;
            seconds = 1;
            this.border = border;
            this.textBlock = textBlock;
            StartNetworkTracking();
        }

        #region Methods
        /// <summary>
        /// Import method
        /// </summary>
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        /// <summary>
        /// Start timer
        /// </summary>
        private void StartNetworkTracking()
        {
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(NetworkStatusUpdate);
            StatusNetworkSetupValue(status);
            Timer.Start();
            Timer.Interval = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Check network
        /// </summary>
        private bool CheckNetworkStatus()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }

        /// <summary>
        /// Set interface state
        /// </summary>
        /// <param name="Status">Network status</param>
        private void StatusNetworkSetupValue(bool Status)
        {
            SolidColorBrush Brush;
            string StatusString;

            if (Status)
            {
                Brush = new SolidColorBrush(Colors.Green);
                StatusString = (string)Application.Current.Resources["descOn"];
            }
            else
            {
                Brush = new SolidColorBrush(Colors.Red);
                StatusString = (string)Application.Current.Resources["descOff"];
            }

            border.Background = Brush;
            textBlock.Text = StatusString;
        }
        #endregion

        #region EventHandler
        /// <summary>
        /// Network status update
        /// </summary>
        private void NetworkStatusUpdate(object sender, EventArgs e)
        {
            bool currentStatus = CheckNetworkStatus();
            if (currentStatus != status)
            {
                status = currentStatus;
                StatusNetworkSetupValue(currentStatus);
            }
        }
        #endregion
    }
}
