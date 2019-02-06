using System.Collections.Generic;
using System.Net;

namespace HttpHeadersViewer.Common
{
    public class QueryBase
    {
        #region Fields
        private int limit;                                          // Request limit
        private List<Request> requests;                             // Request list
        #endregion

        #region Properties
        public int Limit => limit;                                  // Request limit
        public List<Request> Requests => requests;                  // Requests list
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public QueryBase()
        {
            limit = 100;
            requests = new List<Request>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check limit
        /// </summary>
        public bool IsLimit()
        {
            return requests.Count < limit;
        }

        /// <summary>
        /// Converter http headers in new struct
        /// </summary>
        /// <param name="headers">Http headers</param>
        private Dictionary<string, string> HeadersConverter(WebHeaderCollection headers)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] keys = headers.AllKeys;
            int length = keys.Length;
            for(int i = 0; i < length; i++)
            {
                dictionary.Add(keys[i], headers.Get(keys[i]));
            }
            return dictionary;
        }

        /// <summary>
        /// Add query in requests list
        /// </summary>
        /// <param name="url">Url string</param>
        /// <param name="headers">Http headers</param>
        public void AddQuery(string url, WebHeaderCollection headers)
        {
            Dictionary<string, string> collection = HeadersConverter(headers);
            Request request = new Request(url, collection);
            requests.Add(request);
        }
        #endregion

        #region Private Class
        /// <summary>
        /// Requests storage model
        /// </summary>
        public class Request
        {
            #region Fields
            private string url;                                     // Url string                  
            private Dictionary<string, string> headers;             // Http headers
            #endregion

            #region Properties
            public string Url => url;                               // Url string
            public Dictionary<string, string> Headers => headers;   // Http headers
            #endregion

            #region Constructors
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="url">Url string</param>
            /// <param name="headers">Http headers</param>
            public Request(string url, Dictionary<string, string> headers)
            {
                this.url = url;
                this.headers = headers;
            }
            #endregion
        }
        #endregion
    }
}
