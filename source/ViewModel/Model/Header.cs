using System;
using System.Runtime.Serialization;
using HttpHeadersViewer.ViewModel.Base;

namespace HttpHeadersViewer.ViewModel.Model
{
    [DataContract]
    [Serializable]
    public class Header : BaseViewModel
    {
        #region Fields

        private string key;

        private string value;

        #endregion

        #region Constructors

        public Header()
        {

        }

        public Header(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        #endregion

        #region Properties

        [DataMember]
        public string Key
        {
            get => key;
            set
            {
                key = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
