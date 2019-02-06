using System;
using System.Windows;
using WinForms =  System.Windows.Forms;
using HttpHeadersViewer.Common;

namespace HttpHeadersViewer.View
{
    public partial class ExportWindow : Window
    {
        #region Fields
        private Export export;
        private OutputConsole outputConsole;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ExportWindow()
        {
            InitializeComponent();
            ExportPath.Text = "C:\\Users\\" + Environment.UserName + "\\Documents";
        }

        #region Methods
        /// <summary>
        /// Data transfer to the form
        /// </summary>
        /// <param name="queryBase">Data storage model</param>
        /// <param name="selectIndex">Selected item</param>
        public void DataTransferForExport(OutputConsole outputConsole, QueryBase queryBase, int selectIndex)
        {
            export = new Export(outputConsole, queryBase, ExportList);
            this.outputConsole = outputConsole;
            for (int i  = 0; i < queryBase.Requests.Count; i++)
            {
                ExportList.Items.Add(queryBase.Requests[i].Url);
            }
            if (selectIndex != -1)
            {
                ExportList.SelectedIndex = selectIndex;
                SelectedCount.Text = ExportList.SelectedItems.Count + "";
            }
        }

        /// <summary>
        /// Check all export formats
        /// </summary>
        private bool CheckFormats()
        {
            bool[] status = new bool[2];
            status[0] = (bool)f1.IsChecked;
            status[1] = (bool)f2.IsChecked;
            for(int i = 0; i < 2; i++)
            {
                if(status[i] == true)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Form validation
        /// </summary>
        private bool Validation()
        {
            if (CheckFormats())
            {
                MessageBox.Show("Choose export format.", 
                                "Warning",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return false;
            }
            if (ExportList.SelectedIndex == -1)
            {
                MessageBox.Show("Select items from the list to export.",
                                "Warning",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        #endregion

        #region EventHandlers
        /// <summary>
        /// Close export form
        /// </summary>
        private void EventClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Export data
        /// </summary>
        private void EventExport(object sender, RoutedEventArgs e)
        {
            if(Validation())
            {
                if((bool)f1.IsChecked)
                {
                    export.ExportJson(ExportPath.Text);
                }
                if((bool)f2.IsChecked)
                {
                    export.ExportXml(ExportPath.Text);
                }
                outputConsole.Message(ExportPath.Text);
                Close();
            }
        }

        /// <summary>
        /// Select export directory
        /// </summary>
        private void EventSelectCatalog(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog folderDlg = new WinForms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;  
            WinForms.DialogResult result = folderDlg.ShowDialog();
            if (result == WinForms.DialogResult.OK)
            {
                ExportPath.Text = folderDlg.SelectedPath;
            }
        }

        /// <summary>
        /// Select all requests to ExportList
        /// </summary>
        private void EventSelectAllRequests(object sender, RoutedEventArgs e)
        {
            ExportList.SelectAll();
            SelectedCount.Text = ExportList.SelectedItems.Count + "";
        }

        /// <summary>
        /// Unselected all requests to ExportList
        /// </summary>
        private void EventUnSelectAllRequests(object sender, RoutedEventArgs e)
        {
            ExportList.UnselectAll();
            SelectedCount.Text = "";
        }
       
        /// <summary>
        /// Select items to ExportList
        /// </summary>
        private void EventExportList(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int count = ExportList.SelectedItems.Count;
            if(count > 0)
            {
                SelectedCount.Text = ExportList.SelectedItems.Count + "";
            }
            else
            {
                SelectedCount.Text = "";
            }
        }
        #endregion
    }
}
