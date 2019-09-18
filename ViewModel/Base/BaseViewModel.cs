using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace HttpHeadersViewer.ViewModel.Base
{
    [DataContract]
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
