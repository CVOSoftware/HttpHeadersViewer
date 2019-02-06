using System;
using System.Windows.Controls;
using System.Net;

namespace HttpHeadersViewer.Common
{
    /// <summary>
    /// Message type for console output
    /// </summary>
    public enum TypeMessage 
    {
        NetworkError,
        UrlEmpty,
        NotResponse,
        HeadersNotSupport,
        NotHeaders,
        UrlProcessed,
        LimitQuery,
        DeleteItem
    }

    public class OutputConsole
    {
        #region Fields
        private TextBox textBox;                // Link to TextBoxConsole
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textBox"> Link to TextBoxConsole</param>
        public OutputConsole(TextBox textBox)
        {
            this.textBox = textBox;
        }

        #region Methods
        /// <summary>
        /// Converts numeric values ​​to output format
        /// </summary>
        /// <param name="time">Time value</param>
        public string Format(int time)
        {
            string timeFormat = "";
            if (time < 10)
            {
                timeFormat = "0" + time.ToString();
            }
            else
            {
                timeFormat = time.ToString();
            }
            return timeFormat;
        }

        /// <summary>
        /// Gets the formatted string with the current time value
        /// </summary>  
        public string GetCurrentTime()
        {
            DateTime time = DateTime.Now;
            return "[" + Format(time.Hour) + ":" + Format(time.Minute) + ":" + Format(time.Second) + "] ";
        }

        /// <summary>
        /// Outputting a message to the console
        /// </summary>
        /// <param name="UriFormat">Url format</param>
        public void Message(string exportpath)
        {
            textBox.Text = GetCurrentTime() + "Data export to " + exportpath;
        }

        /// <summary>
        /// Outputting a message to the console
        /// </summary>
        /// <param name="UriFormat">Url format</param>
        public void Message(UriFormatException UriFormat)
        {
            textBox.Text = GetCurrentTime() + UriFormat.Message;
        }

        /// <summary>
        /// Outputting a message to the console
        /// </summary>
        /// <param name="httpResponse">Server response</param>
        public void Message(HttpWebResponse httpResponse)
        {
            string message = GetCurrentTime();
            message += "(" + (int)httpResponse.StatusCode + ") ";
            message += httpResponse.StatusCode;
            message += " -" + httpResponse.StatusDescription;
            textBox.Text = message;

        }

        /// <summary>
        /// Outputting a message to the console
        /// </summary>
        /// <param name="type">Message type</param>
        public void Message(TypeMessage type)
        {
            string message = GetCurrentTime();
            switch (type)
            {
                case TypeMessage.NetworkError:
                    message += "Not network connection.";
                    break;
                case TypeMessage.UrlEmpty:
                    message += "Query string is empty.";
                    break;
                case TypeMessage.NotResponse:
                    message += "Response is not.";
                    break;
                case TypeMessage.HeadersNotSupport:
                    message += "Headers is not support.";
                    break;
                case TypeMessage.NotHeaders:
                    message += "Not headers.";
                    break;
                case TypeMessage.UrlProcessed:
                    message += "This URL has already been processed.";
                    break;
                case TypeMessage.LimitQuery:
                    message += "Limit of requests exceeded, remove unused information.";
                    break;
                case TypeMessage.DeleteItem:
                    message += "Кequest information removed.";
                    break;
            }
            textBox.Text = message;
        }

        /// <summary>
        /// Cleaning console output
        /// </summary>
        public void Clear()
        {
            if(textBox.Text != String.Empty)
            {
                textBox.Clear();
            }
        }
        #endregion
    }
}
