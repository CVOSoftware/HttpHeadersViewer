using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using HttpHeadersViewer.Helper;
using Syroot.Windows.IO;
using HttpHeadersViewer.View;
using HttpHeadersViewer.ViewModel.Base;
using HttpHeadersViewer.ViewModel.Model;

namespace HttpHeadersViewer.ViewModel
{
    class ExportViewModel : BaseViewModel
    {
        #region Const

        private enum Format
        {
            json,
            xml
        }

        #endregion

        #region CommandFields

        private RelayCommand getExportPath;

        private RelayCommand export;

        private RelayCommand closeWindow;

        #endregion

        #region ViewModelFields

        private bool jsonFormat;

        private bool xmlFormat;

        private bool selectAll;

        private string exportPath;

        #endregion

        #region Constructor

        public ExportViewModel(ObservableCollection<Request> requests)
        {
            jsonFormat = true;
            xmlFormat = false;
            selectAll = false;
            exportPath = KnownFolders.Documents.Path;
            Requests = requests;
        }

        #endregion

        #region CommandProperties

        public RelayCommand GetExportPathCommand => RelayCommand.Register(ref getExportPath, OnGetExportPath);

        public RelayCommand ExportCommand => RelayCommand.Register(ref export, OnExport, OnCanExecuteExport);

        public RelayCommand CloseWindow => RelayCommand.Register(ref closeWindow, OnCloseWindow);

        #endregion

        #region ViewModelProperties

        public bool JsonFormat
        {
            get => jsonFormat;
            set
            {
                jsonFormat = value;
                OnPropertyChanged();
            }
        }

        public bool XmlFormat
        {
            get => xmlFormat;
            set
            {
                xmlFormat = value;
                OnPropertyChanged();
            }
        }

        public bool SelectAll
        {
            get => selectAll;
            set
            {
                selectAll = value;
                SetRequestIsSelectedProperty(value);
                OnPropertyChanged();
            }
        }

        public string ExportPath
        {
            get => exportPath;
            set
            {
                exportPath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Request> Requests { get; }

        #endregion

        #region CommandExecuteMethods

        private void OnGetExportPath(object parameter)
        {
            var path = DialogHelper.GetDirectoryPath();
            if (path != string.Empty)
            {
                ExportPath = path;
            }
        }

        private void OnExport(object parameter)
        {
            var exportCollection = new ObservableCollection<Request>();
            foreach (var item in Requests)
            {
                if (item.IsSelected)
                {
                    exportCollection.Add(item);
                }
            }

            if (exportCollection.Count > 0)
            {
                if (JsonFormat)
                {
                    var jsonFormatter = new DataContractJsonSerializer(typeof(ObservableCollection<Request>));
                    using (var stream = new FileStream(SavePath(Format.json.ToString()), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        jsonFormatter.WriteObject(stream, Requests);
                    }
                }

                if (XmlFormat)
                {
                    var xmlFormatter = new XmlSerializer(typeof(ObservableCollection<Request>));
                    using (var stream = new FileStream(SavePath(Format.xml.ToString()), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        xmlFormatter.Serialize(stream, Requests);
                    }
                }
            }
            OnCloseWindow(parameter);
        }

        private void OnCloseWindow(object parameter)
        {
            if(parameter is ExportWindow view)
            {
                SetRequestIsSelectedProperty(false);
                view.Close();
            }
        }

        #endregion

        #region CommandCanExecuteMethods

        private bool OnCanExecuteExport(object parameter)
        {
            bool status = false;

            foreach (var item in Requests)
            {
                if (item.IsSelected)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        #endregion

        #region Helpers

        private string SavePath(string format)
        {
            var fileName = $"{DateTime.Now:MM-dd-yyyy hh-mm-ss}.{format}";
            return Path.Combine(exportPath, fileName);
        }

        public void SetRequestIsSelectedProperty(bool value)
        {
            foreach (var item in Requests)
            {
                item.IsSelected = value;
            }
        }

        #endregion
    }
}
