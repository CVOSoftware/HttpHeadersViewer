using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using HttpHeadersViewer.ViewModel.Base;

namespace HttpHeadersViewer.ViewModel.Model
{
    [DataContract]
    [Serializable]
    public class Request : BaseViewModel
    {
        #region Fields

        private bool isSelected;

        private ObservableCollection<Header> headers;

        #endregion

        #region Constructors

        public Request()
        {

        }

        public Request(string requestString)
        {
            isSelected = false;
            RequestString = requestString;
            headers = new ObservableCollection<Header>();
        }

        #endregion

        #region Properties

        [XmlIgnore]
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string RequestString { get; set; }

        [DataMember]
        public ObservableCollection<Header> Headers
        {
            get => headers;
            set
            {
                headers = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
