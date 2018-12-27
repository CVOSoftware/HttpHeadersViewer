using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace HttpHeadersViewer.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebHeaderCollection headers;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Quit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_CreateRequest(object sender, RoutedEventArgs e)
        {
            ButtonDeleteRequest.IsEnabled = true;
            ButtonCreateRequest.IsEnabled = false;
            TextBoxRequestUri.IsEnabled = true;
            TextBoxRequestUri.Focus();
            ButtonGetResponse.IsEnabled = true;
            ComboBoxHeaders.IsEnabled = true;
        }

        private void Button_DeleteRequest(object sender, RoutedEventArgs e)
        {

            ButtonDeleteRequest.IsEnabled = false;
            ButtonCreateRequest.IsEnabled = true;
            TextBoxRequestUri.Clear();
            TextBoxRequestUri.IsEnabled = false;
            ButtonGetResponse.IsEnabled = false;
            ComboBoxHeaders.Items.Clear();
            if (headers != null)
            {
                headers.Clear();
            }
            TextBoxHeaderValue.Clear();
            TextBlockHeadersCount.Text = "0/0";
            ImageGetResponse.ToolTip = "Get response";
        }

        private void Button_GetResponse(object sender, RoutedEventArgs e)
        {
            ButtonGetResponse.IsEnabled = false;
            if (ComboBoxHeaders.Items.Count > 0)
            {
                ComboBoxHeaders.Items.Clear();
                TextBoxHeaderValue.Clear();
            }

            if (TextBoxRequestUri.IsEnabled)
            {
                TextBoxRequestUri.IsEnabled = false;
            }
            HttpWebRequest request = HttpWebRequest.CreateHttp(TextBoxRequestUri.Text);
            WebResponse response = request.GetResponse();
            headers = response.Headers;

            for (int i = 0; i < headers.Count; i++)
            {
                ComboBoxHeaders.Items.Add(headers.GetKey(i));
            }
            ComboBoxHeaders.SelectedIndex = 0;
            ImageGetResponse.ToolTip = "Refresh";
            ButtonGetResponse.IsEnabled = true;
        }

        private void ComboBoxHeaders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlockHeadersCount.Text = 1 + "/" + headers.Count;
            if (ComboBoxHeaders.Items.Count > 0)
            {
                string header = headers.GetKey(ComboBoxHeaders.SelectedIndex);
                TextBoxHeaderValue.Text = headers.Get(header);
            }
        }
    }
}
