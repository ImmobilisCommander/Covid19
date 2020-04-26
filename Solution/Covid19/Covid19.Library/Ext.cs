using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19.Library
{
    internal static class Ext
    {
        private static readonly char[] _trimChars = new char[] { '\"' };

        public static string TryGetValue(this DataRow row, string name)
        {
            string retour = null;

            var s = row[name]?.ToString().Trim(_trimChars);
            if (!string.IsNullOrEmpty(s))
            {
                retour = s;
            }

            return retour;
        }

        public static string TryGetValue(this DataRowView row, string name)
        {
            string retour = null;

            var s = row[name]?.ToString().Trim(_trimChars);
            if (!string.IsNullOrEmpty(s))
            {
                retour = s;
            }

            return retour;
        }

        public static string ToCsv<T>(this IEnumerable<T> data, char separator = ';')
        {
            var retour = string.Empty;

            if (data != null && data.Count() > 0)
            {
                var sb = new StringBuilder();

                var obj = data.FirstOrDefault();

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
    }
}
