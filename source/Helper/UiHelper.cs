using System;
using System.Windows;

namespace HttpHeadersViewer.Helper
{
    internal static class UIHelper
    {
        public static void Update(Action action)
        {
            Application.Current.Dispatcher?.Invoke(action);
        }
    }
}
