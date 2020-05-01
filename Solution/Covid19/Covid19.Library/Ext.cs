using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Covid19.Library
{
    /// <summary>
    /// Extensions class
    /// </summary>
    public static class Ext
    {
        private static readonly char[] _trimChars = new char[] { '\"' };

        /// <summary>
        /// Try get a field value in a row. Returns null if not found
        /// </summary>
        /// <param name="row">The row to look in</param>
        /// <param name="name">The name of the field</param>
        /// <returns></returns>
        internal static string TryGetValue(this DataRow row, string name)
        {
            string retour = null;

            var s = row[name]?.ToString().Trim(_trimChars);
            if (!string.IsNullOrEmpty(s))
            {
                retour = s;
            }

            return retour;
        }

        /// <summary>
        /// Try get a field value in a data row view. Returns null if not found
        /// </summary>
        /// <param name="row">The row to look in</param>
        /// <param name="name">The name of the field</param>
        /// <returns></returns>
        internal static string TryGetValue(this DataRowView row, string name)
        {
            string retour = null;

            var s = row[name]?.ToString().Trim(_trimChars);
            if (!string.IsNullOrEmpty(s))
            {
                retour = s;
            }

            return retour;
        }

        /// <summary>
        /// Generate a CSV string from an enumerable
        /// </summary>
        /// <typeparam name="T">The type in the enumerable</typeparam>
        /// <param name="data">The data from which CSV is generated</param>
        /// <param name="separator">Character to seperate fields</param>
        /// <returns></returns>
        public static string ToCsv<T>(this IEnumerable<T> data, char separator = ';')
        {
            var retour = string.Empty;

            if (data != null && data.Count() > 0)
            {
                var sb = new StringBuilder();

                var props = typeof(T).GetProperties().ToList();

                foreach (var p in props)
                {
                    if (props.IndexOf(p) > 0)
                    {
                        sb.Append($"{separator}{p.Name}");
                    }
                    else
                    {
                        sb.Append($"{p.Name}");
                    }
                }

                sb.AppendLine();

                foreach (var d in data)
                {
                    foreach (var p in props)
                    {
                        if (props.IndexOf(p) > 0)
                        {
                            sb.Append($"{separator}{p.GetValue(d, null)}");
                        }
                        else
                        {
                            sb.Append($"{p.GetValue(d, null)}");
                        }
                    }

                    sb.AppendLine();
                }

                retour = sb.ToString();
            }

            return retour;
        }

        /// <summary>
        /// Clean the text for the location dictionary key
        /// </summary>
        /// <param name="txt">The text to clean</param>
        /// <returns></returns>
        public static string GetKey(this string txt)
        {
            return txt.Replace("\"", "").Replace("_", " ").Trim();
        }

        public static bool DownloadTo(this Uri url, string filePath)
        {
            if (url == null || filePath == null)
            {
                return false;
            }

            using (var wc = new WebClientWithTimeout())
            {
                try
                {
                    wc.DownloadFile(url, filePath);
                    return true;
                }
                catch (WebException)
                {
                    return false;
                }
            }
        }
    }
}
