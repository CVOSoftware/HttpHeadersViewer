using System.Windows;
using HttpHeadersViewer.View;
using HttpHeadersViewer.ViewModel;

namespace HttpHeadersViewer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if(this.IsOneTimeLaunch())
            {
                base.OnStartup(e);
                var viewModel = new MainViewModel();
                var view = new MainWindow
                {
                    DataContext = viewModel
                };
                view.Show();
            }
            else
            {
                this.Shutdown();
            }
        }
    }
}
