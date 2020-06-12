using HttpHeadersViewer.Helper;
using HttpHeadersViewer.View;
using HttpHeadersViewer.ViewModel.Base;
using HttpHeadersViewer.ViewModel.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace HttpHeadersViewer.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        #region Const

        public const int Limit = 100;

        #endregion

        #region CommandFields

        private RelayCommand exitProgram;

        private RelayCommand showExportWindow;

        private RelayCommand removeRequest;

        private RelayCommand addRequest;

        private RelayCommand startRequest;

        #endregion

        #region ViewModelFields

        private bool blockedRequestField;

        private bool awaitingAnswerRequest;

        private string requestString;

        private string requestCount;

        private string headerCount;

        private string headerValue;

        private string infoMessage;

        private string networkStatus;
        
        private Request selectRequest;

        private Header selectHeaderIndex;

        #endregion

        #region HelperFields

        private bool switchRequest;     

        #endregion

        #region Constructor

        public MainViewModel()
        {
            blockedRequestField = false;
            awaitingAnswerRequest = false;
            switchRequest = true;
            requestString = string.Empty;
            requestCount = string.Empty;
            HeaderCount = string.Empty;
            headerValue = string.Empty;
            infoMessage = string.Empty;
            networkStatus = NetworkHelper.Label;
            selectRequest = null;
            selectHeaderIndex = null;
            Requests = new ObservableCollection<Request>();
            NetworkHelper.StartNetworkTracking(update => NetworkStatus = update);
        }

        #endregion

        #region CommandProperties

        public RelayCommand ExitProgram => RelayCommand.Register(ref exitProgram, OnExitProgram);

        public RelayCommand ShowExportWindow => RelayCommand.Register(ref showExportWindow, OnShowExportWindow, CanExport);

        public RelayCommand RemoveRequest => RelayCommand.Register(ref removeRequest, OnRemoveRequest, CanRemove);

        public RelayCommand AddRequest => RelayCommand.Register(ref addRequest, OnAddRequest, CanAddRequest);

        public RelayCommand StartRequest => RelayCommand.Register(ref startRequest, OnStartRequest, CanStartRequest);
        
        #endregion

        #region ViewModelProperties

        public bool BlockedRequestFields
        {
            get => blockedRequestField;
            set
            {
                blockedRequestField = value;
                OnPropertyChanged();
            }
        }

        public bool AwaitingAnswerRequest
        {
            get => awaitingAnswerRequest;
            set
            {
                awaitingAnswerRequest = value;
                OnPropertyChanged();
            }
        }
        
        public string RequestString
        {
            get => requestString;
            set
            {
                requestString = value;
                if(switchRequest)
                {
                    foreach (var request in Requests)
                    {
                        if (request.RequestString == requestString)
                        {
                            BlockedRequestFields = false;
                            SelectRequest = request;
                            InfoMessage = string.Empty;
                            break;
                        }
                    }
                }
                OnPropertyChanged();
            }
        }
        
        public string RequestCount
        {
            get => requestCount;
            set
            {
                requestCount = value;
                OnPropertyChanged();
            }
        }

        public string HeaderCount
        {
            get => headerCount;
            set
            {
                headerCount = value;
                OnPropertyChanged();
            }
        }

        public string HeaderValue
        {
            get => headerValue;
            set
            {
                headerValue = value;
                OnPropertyChanged();
            }
        }
        
        public string InfoMessage
        {
            get => infoMessage;
            set
            {
                infoMessage = value == string.Empty ? value : ConsoleHelper.OutputFormat(value);
                OnPropertyChanged();
            }
        }

        public string NetworkStatus
        {
            get => networkStatus;
            set
            {
                networkStatus = value;
                OnPropertyChanged();
            }
        }

        public Request SelectRequest
        {
            get => selectRequest;
            set
            {
                selectRequest = value;
                switchRequest = false;
                RequestString = selectRequest?.RequestString ?? string.Empty;
                switchRequest = true;
                RequestCount = Counter(Requests.IndexOf(value), Requests.Count);
                InfoMessage = string.Empty;
                if (value != null)
                {
                    BlockedRequestFields = false;
                    SelectHeader = value.Headers?.First();
                }
                OnPropertyChanged();
            }
        }

        public Header SelectHeader
        {
            get => selectHeaderIndex;
            set
            {
                selectHeaderIndex = value;
                if(value != null)
                {
                    HeaderValue = value.Value;
                    HeaderCount = Counter(SelectRequest.Headers.IndexOf(value), SelectRequest.Headers.Count);
                }
                InfoMessage = string.Empty;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Request> Requests { get; }
        
        #endregion

        #region CommandExecuteMethods

        private void OnExitProgram(object parameter)
        {
            if(parameter is MainWindow view)
            {
                view.Close();
            }
        }

        private void OnShowExportWindow(object parameter)
        {
            if(parameter is MainWindow parentView)
            {
                var viewModel = new ExportViewModel(Requests);
                var childView = new ExportWindow
                {
                    DataContext = viewModel,
                    ShowInTaskbar = false,
                    Owner = parentView
                };
                InfoMessage = string.Empty;
                childView.Closing += (IChannelSender, e) =>
                {
                    viewModel.SetRequestIsSelectedProperty(false);
                };
                childView.ShowDialog();
            }
        }

        private void OnRemoveRequest(object parameter)
        {
            Requests.Remove(selectRequest);
            RequestCount = Counter(Requests.IndexOf(selectRequest), Requests.Count);
            BlockedRequestFields = false;
            RequestString = string.Empty;
            HeaderCount = string.Empty;
            HeaderValue = string.Empty;
            InfoMessage = string.Empty;
        }

        private void OnAddRequest(object parameter)
        {
            BlockedRequestFields = !blockedRequestField || requestString != string.Empty;
            SelectRequest = null;
        }
        
        private void OnStartRequest(object parameter)
        {
            void UpdateHeaders(NameValueCollection response, Request request)
            {
                request.Headers.Clear();
                for (var i = 0; i < response.Count; i++)
                {
                    var header = new Header(response.GetKey(i), response.Get(i));
                    request.Headers.Add(header);
                }
            }
            ThreadPool.QueueUserWorkItem(_ => {
                BlockedRequestFields = false;
                AwaitingAnswerRequest = true;
                RequestHelper.Request(requestString,
                (response) => {
                    var newRequest = true;
                    foreach(var request in Requests)
                    {
                        if (requestString != request.RequestString) continue;
                        UIHelper.Update(() =>
                        {
                            UpdateHeaders(response, request);
                            SelectHeader = request.Headers?.First();
                        });
                        newRequest = false;
                        break;
                    }
                    if(newRequest)
                    {
                        var request = new Request(requestString);
                        UpdateHeaders(response, request);
                        UIHelper.Update(() =>
                        {
                            Requests.Add(request);
                            SelectRequest = request;
                        });
                    }
                    InfoMessage = ConsoleHelper.Message(MessageType.Success);
                },
                (message) =>
                {
                    InfoMessage = message;
                    BlockedRequestFields = true;
                    RequestString = string.Empty;
                });
                AwaitingAnswerRequest = false;
                UIHelper.Update(RelayCommand.RaiseCanExecuteChanged);
            });
        }

        #endregion

        #region CommandCanExecuteMethods

        private bool CanAddRequest(object parameter)
        {
            return !awaitingAnswerRequest
                   && Requests.Count < Limit;
        }

        private bool CanExport(object parameter)
        {
            return Requests.Count > 0 && awaitingAnswerRequest == false;
        }

        private bool CanRemove(object parameter)
        {
            return Requests.Count > 0 
                && awaitingAnswerRequest == false 
                && selectRequest != null;
        }

        private bool CanStartRequest(object parameter)
        {
            return NetworkHelper.Status
                   && awaitingAnswerRequest == false
                   && (blockedRequestField || requestString == selectRequest?.RequestString) 
                   && requestString != string.Empty;
        }
        
        #endregion

        #region Helpers

        private static string Counter(int current, int all)
        {
            return current < 0 ? 
                (all > 0 ? all.ToString() : string.Empty) : $"{++current} / {all}";
        }
        
        #endregion
    }
}
