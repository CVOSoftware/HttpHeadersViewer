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
    public partial class MainWindow : Window
    {
        public List<WebHeaderCollection> headers;

        public string GetCurrentTime()
        {
            DateTime Now = DateTime.Now;
            return "[" + Now.Hour + ":" + Now.Minute + ":" + Now.Second + "] ";
        }

        public void PrintComboBoxHeaders()
        {
            for (int i = 0; i < headers[ListBoxRequests.SelectedIndex].Count; i++)
            {
                ComboBoxHeaders.Items.Add(headers[ListBoxRequests.SelectedIndex].GetKey(i));
            }
            ComboBoxHeaders.SelectedIndex = 0;
        }
        
        public MainWindow()
        {
            InitializeComponent();
            headers = new List<WebHeaderCollection>();
            TextBlockStatus.Text = "Programm ready";
        }

        private void ButtonQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            
            headers.RemoveAt(ListBoxRequests.SelectedIndex);
            ListBoxRequests.Items.RemoveAt(ListBoxRequests.SelectedIndex);
            TextBoxRequest.Clear();
            TextBlockRequests.Text = "Requests " + ListBoxRequests.Items.Count;
            ButtonDelete.IsEnabled = false;
            ComboBoxHeaders.Items.Clear();
            TextBlockHeaders.Text = "Headers 0";
            TextBoxHeader.Clear();
        }
        
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if(headers.Count > 0)
            {
                ButtonDelete.IsEnabled = false;
                TextBlockRequests.Text = "Requests " + ListBoxRequests.Items.Count;
                ListBoxRequests.UnselectAll();
                ComboBoxHeaders.Items.Clear();
                TextBlockHeaders.Text = "Headers 0";
                TextBoxHeader.Clear();
            }
            TextBoxRequest.IsEnabled = true;
            TextBoxRequest.Clear();
            TextBoxRequest.Focus();
            ButtonResponse.IsEnabled = true;
        }

        private void ButtonResponse_Click(object sender, RoutedEventArgs e)
        {
            if(TextBoxRequest.Text.Length > 0)
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(TextBoxRequest.Text);
                WebResponse response = request.GetResponse();
                headers.Add(response.Headers);
                ListBoxRequests.Items.Add(TextBoxRequest.Text);
                ListBoxRequests.SelectedIndex = ListBoxRequests.Items.Count - 1;
                TextBoxRequest.IsEnabled = false;
                TextBoxConsole.Clear();
                ButtonResponse.IsEnabled = false;
                TextBlockRequests.Text = "Requests " + ListBoxRequests.Items.Count + "/" + ListBoxRequests.Items.Count;
                if(headers.Count > 0)
                {
                    ComboBoxHeaders.Items.Clear();
                }
                PrintComboBoxHeaders();
            }
            else
            {
                TextBoxRequest.Focus();
                TextBoxConsole.Text = GetCurrentTime() + "Request string is empty.\n";
            }
        }

        private void ListBoxRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListBoxRequests.SelectedItem != null)
            {
                ButtonDelete.IsEnabled = true;
                TextBoxRequest.Text = ListBoxRequests.SelectedValue.ToString();
                TextBoxRequest.IsEnabled = false;
                TextBlockRequests.Text = "Requests " + (ListBoxRequests.SelectedIndex + 1) + "/" + ListBoxRequests.Items.Count;
                ComboBoxHeaders.Items.Clear();
                PrintComboBoxHeaders();
            }
        }

        private void ComboBoxHeaders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            if(ComboBoxHeaders.Items.Count > 0)
            {
                TextBlockHeaders.Text = "Headres " + (ComboBoxHeaders.SelectedIndex + 1) + "/" + ComboBoxHeaders.Items.Count;
                TextBoxHeader.Text = headers[ListBoxRequests.SelectedIndex].Get(ComboBoxHeaders.SelectedIndex);
            }
        }
    }
}
