using System;
using System.Windows.Controls;
using System.IO;

namespace HttpHeadersViewer.Common
{
    internal class Export
    {
        #region Fields
        private OutputConsole outputConsole;                      // Link to outputConsole
        private QueryBase queryBase;                              // Link to queryBase
        private ListBox exportList;                               // Link to exportList
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputConsole">Link to outputConsole</param>
        /// <param name="queryBase">Link to queryBase</param>
        /// <param name="exportList">Link to exportList</param>
        public Export(OutputConsole outputConsole, QueryBase queryBase, ListBox exportList)
        {
            this.outputConsole = outputConsole;
            this.queryBase = queryBase;
            this.exportList = exportList;
        }

        #region Methods
        /// <summary>
        /// Creates a date prefix string
        /// </summary>
        private string DatePrefix()
        {
            DateTime date = DateTime.Now;
            string day = outputConsole.Format(date.Day),
                   mounth = outputConsole.Format(date.Month),
                   hour = outputConsole.Format(date.Hour),
                   minute = outputConsole.Format(date.Minute);
            return String.Format(" {0}-{1}-{2} {3}-{4}", day, mounth, date.Year, hour, minute);
        }

        /// <summary>
        /// Forms a complete export path
        /// </summary>
        /// <param name="exportPath">Export path</param>
        /// <param name="type">Format type</param>
        private string CreateFullName(string exportPath, string type)
        {
            return exportPath + "\\HttpHeaders" + DatePrefix() + "." + type;
        }

        /// <summary>
        /// Creates file
        /// </summary>
        /// <param name="fullName">Export path</param>
        /// <param name="exportData">Data</param>
        private void Write(string fullName, string exportData)
        {
            using (FileStream file = new FileStream(fullName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.WriteLine(exportData);
                    writer.Close();
                }
                file.Close();
            }
        }

        /// <summary>
        /// Creates an export template in json format
        /// </summary>
        /// <returns></returns>
        private string JsonData()
        {
            int count = exportList.SelectedItems.Count;
            string temp = "{\n";

            if(count > 1)
            {
                int j = 1;

                temp += "\t\"count\": \"" + count + "\",\n";
                temp += "\t\"data\": [\n";
                foreach (var item in exportList.SelectedItems)
                {
                    int index = exportList.SelectedItems.IndexOf(item),
                        length = queryBase.Requests[index].Headers.Count - 1,
                        i = 0;

                    temp += "\t\t{\n";
                    temp += "\t\t\t\"url\": \"" + queryBase.Requests[index].Url + "\",\n";
                    temp += "\t\t\t\"headers\": {\n";
                    foreach (var item2 in queryBase.Requests[index].Headers)
                    {
                        temp += "\t\t\t\t\"" + item2.Key + "\": " + "\"" + item2.Value.Replace("\"", "'") + "\"";
                        if (i < length)
                        {
                            temp += ",";
                        }
                        temp += "\n";
                        i++;
                    }
                    temp += "\t\t\t}\n\t\t}";
                    if (j < count)
                    {
                        temp += ",";
                    }
                    temp += "\n";
                    j++;
                }
                temp += "\t]\n";
            }
            else
            {
                int index = exportList.SelectedIndex,
                    length = queryBase.Requests[index].Headers.Count - 1,
                    i = 0;

                temp += "\t\"url\": \"" + queryBase.Requests[index].Url + "\",\n";
                temp += "\t\"headers\": {\n";
                foreach (var item in queryBase.Requests[index].Headers)
                {
                    temp += "\t\t\"" + item.Key + "\": " + "\"" + item.Value.Replace("\"", "'") + "\"";
                    if(i < length)
                    {
                        temp += ",";
                    }
                    temp += "\n";
                    i++;
                }
                temp += "\t}\n";
            }
            temp += "}";

            return temp;
        }

        /// <summary>
        /// Creates an export template in xml format
        /// </summary>
        /// <returns></returns>
        private string XmlData()
        {
            int count = exportList.SelectedItems.Count;
            string temp = "<?xml version=\"1.1\" encoding=\"UTF - 8\" ?>\n";

            temp += "<head>\n";
            if (count > 1)
            {
                temp += "\t<count>" + count + "<count>\n";
                temp += "\t<headers>\n";
                foreach (var item in exportList.SelectedItems)
                {
                    int index = exportList.SelectedItems.IndexOf(item);
                    temp += "\t\t<data>\n";
                    temp += "\t\t\t<url>" + queryBase.Requests[index].Url + "</url>\n";
                    foreach (var item2 in queryBase.Requests[index].Headers)
                    {
                        temp += "\t\t\t<header type=\"" + item2.Key + "\">" + item2.Value + "</header>\n";
                    }
                    temp += "\t\t</data>\n";
                }
                temp += "\t</header>\n";
            }
            else
            {
                int index = exportList.SelectedIndex;

                temp += "\t<url>" + queryBase.Requests[index].Url + "</url>\n";
                foreach (var item in queryBase.Requests[index].Headers)
                {
                    temp += "\t<header type=\"" + item.Key + "\">" + item.Value + "</header>\n";
                }
            }
            temp += "</head>";

            return temp;
        }
        
        /// <summary>
        /// Export to json format
        /// </summary>
        /// <param name="exportPath">Data</param>
        public void ExportJson(string exportPath)
        {
            string fullName = CreateFullName(exportPath, "json"),
                   exportData = JsonData();
            Write(fullName, exportData);
        }

        /// <summary>
        /// Export to xml format
        /// </summary>
        /// <param name="exportPath">Data</param>
        public void ExportXml(string exportPath)
        {
            string fullName = CreateFullName(exportPath, "xml"),
                   exportData = XmlData();
            Write(fullName, exportData);
        }
        #endregion
    }
}
