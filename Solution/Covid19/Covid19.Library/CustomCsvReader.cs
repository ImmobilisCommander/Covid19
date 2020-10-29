// <copyright file="CustomCsvReader.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using CsvHelper;
using System;
using System.Globalization;
using System.IO;

namespace Covid19.Library
{
    /// <summary>
    /// Custom implementation of <see cref="CsvReader"/>
    /// </summary>
    internal class CustomCsvReader : CsvReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sr">Stream of the file</param>
        /// <param name="inf">Culture of the file</param>
        public CustomCsvReader(StreamReader sr, CultureInfo inf)
            : base(sr, inf)
        { }

        /// <summary>
        /// Get the filed by index if index greater than or equal to 0
        /// </summary>
        /// <param name="index">Index of the field to get</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the filed by index if index greater than or equal to 0 and convert value to int
        /// </summary>
        /// <param name="index">Index of the field to get</param>
        /// <returns></returns>
        public int GetFieldAsInt(int index, CultureInfo cu)
        {
            if (index >= 0)
            {
                var temp = base.GetField(index);
                if (string.IsNullOrEmpty(temp))
                {
                    return 0;
                }
                else if (int.TryParse(temp, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, cu, out int result))
                {
                    return result;
                }
                else
                {
                    throw new FormatException($"Field {index} is not in correct format: {temp}");
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Get the filed by index if index greater than or equal to 0 and convert value to double
        /// </summary>
        /// <param name="index">Index of the field to get</param>
        /// <param name="cu">Culture of the value of the field</param>
        /// <returns></returns>
        public double GetFieldAsDouble(int index, CultureInfo cu)
        {
            if (index >= 0)
            {
                var temp = base.GetField(index);
                if (string.IsNullOrEmpty(temp))
                {
                    return 0;
                }
                else if (double.TryParse(temp, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, cu, out double result))
                {
                    return result;
                }
                else
                {
                    throw new FormatException($"Field {index} is not in correct format: {temp}");
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Get the filed by name
        /// </summary>
        /// <param name="names">Array of name to test</param>
        /// <returns></returns>
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
