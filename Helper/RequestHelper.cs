using System;
using System.Net;

namespace HttpHeadersViewer.Helper
{
    internal static class RequestHelper
    {
        public static void Request(string requestString, Action<WebHeaderCollection> saccess, Action<string> error)
        {
            HttpWebRequest request;
            try
            {
                request = WebRequest.CreateHttp(requestString);
                using (var response = request.GetResponse())
                {
                    saccess(response.Headers);
                }
            }
            catch (Exception e)
            {
                error(e.Message);
            }
        }
    }
}
