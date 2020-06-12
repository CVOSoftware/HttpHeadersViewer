using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace HttpHeadersViewer
{
    internal static class ApplicationLaunch
    {
        private static Mutex mutex;

        public static bool IsOneTimeLaunch(this Application application, string uniqueName = null)
        {
            var applicationName = Path.GetFileName(Assembly.GetEntryAssembly()?.GetName().Name);
            uniqueName = uniqueName ?? $"{Environment.MachineName}_{Environment.UserName}_{applicationName}";
            application.Exit += OnExit;
            mutex = new Mutex(true, uniqueName, out var isOneTimeLaunch);
            return isOneTimeLaunch;
        }

        private static void OnExit(object sender, EventArgs e)
        {
            mutex.Dispose();
            mutex = null;
        }
    }
}
