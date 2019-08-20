using System.Windows;
using HttpHeadersViewer.View;
using HttpHeadersViewer.ViewModel;

namespace HttpHeadersViewer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainViewModel viewModel = new MainViewModel();
            MainWindow view = new MainWindow();
            view.DataContext = viewModel;
            view.Show();
        }
    }
}
