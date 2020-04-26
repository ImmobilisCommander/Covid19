using CsvHelper;
using System;
using System.Globalization;
using System.IO;

namespace Covid19.Library
{
    internal class CustomCsvReader : CsvReader
    {
        public CustomCsvReader(StreamReader sr, CultureInfo inf)
            : base(sr, inf)
        { }

        public override string GetField(string name)
        {
            var temp = base.GetField(name);
            return string.IsNullOrEmpty(temp) ? null : temp;
        }

        public override string GetField(int index)
        {
            if (index >= 0)
            {
                return base.GetField(index);
            }
            else
            {
                return null;
            }
        }

        public int GetFieldAsInt(int index)
        {
            if (index >= 0)
            {
                var temp = base.GetField(index);
                return string.IsNullOrEmpty(temp) ? 0 : Convert.ToInt32(temp);
            }
            else
            {
                return 0;
            }
        }

        public double GetFieldAsDouble(int index, CultureInfo cu)
        {
            if (index >= 0)
            {
                var temp = base.GetField(index);
                return string.IsNullOrEmpty(temp) ? 0 : Convert.ToDouble(temp, cu);
            }
            else
            {
                return 0;
            }
        }

        public string GetField(string[] names)
        {
            string temp = null;
            foreach (var name in names)
            {
                if (base.TryGetField(name, out temp))
                {
                    break;
                }
            }
            return string.IsNullOrEmpty(temp) ? null : temp;
        }
    }
}
