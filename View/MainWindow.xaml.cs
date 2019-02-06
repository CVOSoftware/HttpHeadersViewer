using System;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using HttpHeadersViewer.Common;

namespace HttpHeadersViewer.View
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// ButtonAdd status enumeration
        /// </summary>
        private enum StateAdd
        {
            Off,
            On
        }

        #region Fields
        private StateAdd stateAdd;              // ButtonAdd State
        private OutputConsole outputConsole;    // Error and warning message output in console
        private StatusNetwork statusNetwork;    // Тetwork connectivity check
        private QueryBase queryBase;            // Data storage model
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            stateAdd = StateAdd.Off;
            outputConsole = new OutputConsole(TextBoxConsole);
            statusNetwork = new StatusNetwork(NetworkStatusIcon, TextBlockStatus);
            queryBase = new QueryBase();
            TextBlockLimit.Text = String.Format("{0}", queryBase.Limit);
        }

        #region Methods
        /// <summary>
        /// Counter
        /// </summary>
        /// <param name="count">Number of requests</param>
        private string FormatQueriesInfo(int count)
        {
            return count == 0 ? "" : count + "";
        }

        /// <summary>
        /// Counter
        /// </summary>
        /// <param name="index">Select item</param>
        /// <param name="all">All requests</param>
        private string FormatQueriesInfo(int index, int all)
        {
            return index + "/" + all;
        }

        /// <summary>
        /// Check repeat url
        /// </summary>
        /// <param name="url">Url string</param>
        private bool IsUrl(string url)
        {
            int count = queryBase.Requests.Count;
            for(int i = 0; i < count; i++)
            {
                if (queryBase.Requests[i].Url == url)
                {
                    ListBoxQueries.SelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Сheck before requests
        /// </summary>
        /// <param name="url">Url string</param>
        private bool Validation(string url)
        {
            if (!statusNetwork.Status)
            {
                outputConsole.Message(TypeMessage.NetworkError);
                TextBoxUrl.IsEnabled = true;
                ButtonResponse.IsEnabled = true;
                TextBoxUrl.Focus();
                return false;
            }
            if (url == String.Empty)
            {
                outputConsole.Message(TypeMessage.UrlEmpty);
                TextBoxUrl.IsEnabled = true;
                ButtonResponse.IsEnabled = true;
                TextBoxUrl.Focus();
                return false;
            }
            if(IsUrl(url))
            {
                outputConsole.Message(TypeMessage.UrlProcessed);
                TextBoxUrl.IsEnabled = false;
                ButtonResponse.IsEnabled = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Http request and its processing
        /// </summary>
        /// <param name="url">Url string</param>
        private bool HttpRequest(string url)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        if (!response.SupportsHeaders)
                        {
                            outputConsole.Message(TypeMessage.HeadersNotSupport);
                            return false;
                        }
                        if (response.Headers.Count == 0)
                        {
                            outputConsole.Message(TypeMessage.NotHeaders);
                            return false;
                        }
                        queryBase.AddQuery(url, response.Headers);
                        return true;
                    }
                }
                catch (WebException ex)
                {
                    WebExceptionStatus status = ex.Status;
                    if (status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                        outputConsole.Message(httpResponse);
                    }
                }
            }
            catch (UriFormatException fex)
            {
                outputConsole.Message(fex);
            }
            return false;
        }

        /// <summary>
        /// Add Items to ListBoxQueries
        /// </summary>
        /// <param name="index">New item index</param>
        private void AddToListBox(int index)
        {
            string url = queryBase.Requests[index].Url;
            ListBoxQueries.Items.Add(url);
            ListBoxQueries.SelectedIndex = index;
            ListBoxQueries.ScrollIntoView(ListBoxQueries.SelectedItem);
        }

        /// <summary>
        /// Add items to ComboBoxHeaders
        /// </summary>
        /// <param name="index">Selected item to ListBoxQueries</param>
        private void AddToComboBox(int index)
        {
            ComboBoxHeaders.Items.Clear();
            foreach(var value in queryBase.Requests[index].Headers.Keys)
            {
                ComboBoxHeaders.Items.Add(value);
            }
            ComboBoxHeaders.SelectedIndex = 0;
        }

        /// <summary>
        /// Add header value to TextBoxHeaders
        /// </summary>
        /// <param name="indexList">Selected item to ListBoxQueries</param>
        /// <param name="indexCombo">Selected item to ComboBoxHeaders</param>
        private void AddToTextBoxHeader(int indexList, int indexCombo)
        {
            string key = ComboBoxHeaders.Items[indexCombo].ToString();
            TextBoxHeader.Text = queryBase.Requests[indexList].Headers[key];
        }
        #endregion

        #region EventHandler
        /// <summary>
        /// Close the programm
        /// </summary>
        private void QuitEvent(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Export information
        /// </summary>
        private void ExportEvent(object sender, RoutedEventArgs e)
        {
            ExportWindow exportWindow;
            stateAdd = StateAdd.Off;
            ButtonAdd.ToolTip = Application.Current.Resources["descAdd"];
            TextBoxUrl.IsEnabled = false;
            ButtonResponse.IsEnabled = false;
            TextBoxUrl.Clear();
            outputConsole.Clear();
            exportWindow = new ExportWindow();
            exportWindow.Owner = this;
            exportWindow.ShowInTaskbar = false;
            exportWindow.DataTransferForExport(outputConsole, queryBase, ListBoxQueries.SelectedIndex);
            exportWindow.ShowDialog();
        }

        /// <summary>
        /// Delete record 
        /// </summary>
        private void DeleteEvent(object sender, RoutedEventArgs e)
        {
            int index = ListBoxQueries.SelectedIndex;
            if(index != -1)
            {
                TextBoxUrl.Clear();
                ListBoxQueries.Items.RemoveAt(index);
                ComboBoxHeaders.Items.Clear();
                TextBoxHeader.Clear();
                queryBase.Requests.RemoveAt(index);
                TextBlockRequests.Text = FormatQueriesInfo(queryBase.Requests.Count);
                TextBlockHeaders.Text = "";
                ButtonDelete.IsEnabled = false;
                if(queryBase.Requests.Count == 0)
                {
                    ButtonExport.IsEnabled = false;
                }
                outputConsole.Message(TypeMessage.DeleteItem);
            }
        }

        /// <summary>
        /// Add new request item
        /// </summary>
        private void AddEvent(object sender, RoutedEventArgs e)
        {
            ButtonDelete.IsEnabled = false;
            ListBoxQueries.UnselectAll();
            ComboBoxHeaders.Items.Clear();
            TextBlockRequests.Text = FormatQueriesInfo(queryBase.Requests.Count);
            TextBlockHeaders.Text = "";
            TextBoxHeader.Clear();
            outputConsole.Clear();
            if(queryBase.IsLimit())
            {
                switch (stateAdd)
                {
                    case StateAdd.Off:
                        stateAdd = StateAdd.On;
                        ButtonAdd.ToolTip = Application.Current.Resources["descClose"];
                        TextBoxUrl.IsEnabled = true;
                        ButtonResponse.IsEnabled = true;
                        TextBoxUrl.Focus();
                        break;
                    case StateAdd.On:
                        stateAdd = StateAdd.Off;
                        ButtonAdd.ToolTip = Application.Current.Resources["descAdd"];
                        TextBoxUrl.IsEnabled = false;
                        ButtonResponse.IsEnabled = false;
                        break;
                }
            }
            else 
            {
                outputConsole.Message(TypeMessage.LimitQuery);
            }
            TextBoxUrl.Clear();
        }

        /// <summary>
        /// Scroll to end
        /// </summary>
        private void TextBoxUrlEvent(object sender, TextChangedEventArgs e)
        {
            TextBoxUrl.ScrollToEnd();
        }

        /// <summary>
        /// Sending request
        /// </summary>
        private void ResponseEvent(object sender, RoutedEventArgs e)
        {
            ButtonAdd.IsEnabled = false;
            TextBoxUrl.IsEnabled = false;
            ButtonResponse.IsEnabled = false;
            TextBoxUrl.ScrollToEnd();
            if(Validation(TextBoxUrl.Text))
            {
                if (HttpRequest(TextBoxUrl.Text))
                {
                    int index = queryBase.Requests.Count - 1;
                    AddToListBox(index);
                    AddToComboBox(index);
                    AddToTextBoxHeader(index, 0);
                    TextBlockRequests.Text = FormatQueriesInfo(index + 1, index + 1);
                    TextBlockHeaders.Text = FormatQueriesInfo(1, queryBase.Requests[index].Headers.Count);
                    stateAdd = StateAdd.Off;
                    ButtonAdd.ToolTip = Application.Current.Resources["descAdd"];
                    ButtonExport.IsEnabled = true;
                    ButtonDelete.IsEnabled = true;
                    outputConsole.Clear();
                }
                else
                {
                    TextBoxUrl.IsEnabled = true;
                    ButtonResponse.IsEnabled = true;
                    TextBoxUrl.Focus();
                }
            }
            ButtonAdd.IsEnabled = true;
        }

        /// <summary>
        /// Selection item to ListBoxQuaries
        /// </summary>
        private void SelectQueryEvent(object sender, SelectionChangedEventArgs e)
        {
            int index = ListBoxQueries.SelectedIndex;
            if(index != -1)
            {
                TextBoxUrl.Text = queryBase.Requests[index].Url;
                TextBlockRequests.Text = FormatQueriesInfo(index + 1, queryBase.Requests.Count);
                TextBlockHeaders.Text = FormatQueriesInfo(1, queryBase.Requests[index].Headers.Count);
                AddToComboBox(index);
                AddToTextBoxHeader(index, 0);
                ButtonDelete.IsEnabled = true;
            }
            if(stateAdd == StateAdd.Off)
            {
                TextBoxUrl.IsEnabled = false;
                ButtonResponse.IsEnabled = false;
            }
            outputConsole.Clear();
        }

        /// <summary>
        /// Selection item to ComboBoxHeaders
        /// </summary>
        private void SelectHeaderEvent(object sender, SelectionChangedEventArgs e)
        {
            int indexList = ListBoxQueries.SelectedIndex;
            if(indexList != -1)
            {
                int indexCombo = ComboBoxHeaders.SelectedIndex;
                if(indexCombo != -1)
                {
                    AddToTextBoxHeader(indexList, indexCombo);
                    TextBlockHeaders.Text = FormatQueriesInfo(indexCombo + 1, queryBase.Requests[indexList].Headers.Count);
                }
            }
            if (stateAdd == StateAdd.On)
            {
                stateAdd = StateAdd.Off;
                ButtonAdd.ToolTip = Application.Current.Resources["descAdd"];
                TextBoxUrl.IsEnabled = false;
                ButtonResponse.IsEnabled = false;
            }
        }
        #endregion
    }
}
