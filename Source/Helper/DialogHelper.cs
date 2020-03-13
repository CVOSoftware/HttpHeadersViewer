using System.Windows.Forms;

namespace HttpHeadersViewer.Helper
{
    internal static class DialogHelper
    {
        public static string GetDirectoryPath()
        {
            var defaultPath = string.Empty;
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    defaultPath = dialog.SelectedPath;
                }
            }

            return defaultPath;
        }
    }
}
