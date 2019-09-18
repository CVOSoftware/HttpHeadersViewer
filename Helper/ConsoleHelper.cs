using System;

namespace HttpHeadersViewer.Helper
{
    internal enum MessageType
    {
        Default,
        Test,
        Success
    };

    internal static class ConsoleHelper
    {
        public static string Message(MessageType messageType)
        {
            var message = string.Empty;
            switch(messageType)
            {
                case MessageType.Test:
                    message = "Test";
                    break;
                case MessageType.Success:
                    message = "Successful request";
                    break;
                case MessageType.Default:
                    break;
            }
            return message;
        }

        public static string OutputFormat(string message)
        {
            return $"[{DateTime.Now:HH:mm:ss}] {message}";
        }
    }
}
