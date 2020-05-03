// <copyright file="RawData.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using System;
using System.Globalization;

namespace Covid19.Library
{
    /// <summary>
    /// Object that olds data and that will be used to save data as CSV
    /// </summary>
    public class RawData
    {
        public RawData(string line)
        {
            var arr = line.Split(';');
            this.DataProvider = arr[0];
            this.Id = Convert.ToInt32(arr[1]);
            this.Area = arr[2];
            this.SubArea = arr[3];
            this.Admin2 = arr[4];
            this.Date = Convert.ToDateTime(arr[5], CultureInfo.CreateSpecificCulture("fr-FR"));
            this.Confirmed = Convert.ToInt32(arr[6]);
            this.Death = Convert.ToInt32(arr[7]);
            this.NewConfirmed = Convert.ToInt32(arr[8]);
            this.NewDeath = Convert.ToInt32(arr[9]);
            this.Latitude = Convert.ToDouble(arr[10]);
            this.Longitude = Convert.ToDouble(arr[11]);
        }

        public RawData()
        {
            this.Confirmed = 0;
            this.Death = 0;
            this.Latitude = 0D;
            this.Longitude = 0D;
        }

        /// <summary>
        /// Name of the data provider
        /// </summary>
        public string DataProvider { get; set; }
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Region/country
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// Sub area
        /// </summary>
        public string SubArea { get; set; }
        /// <summary>
        /// Administrative location
        /// </summary>
        public string Admin2 { get; set; }
        /// <summary>
        /// Date of the data
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Effective confirmed cases
        /// </summary>
        public int Confirmed { get; set; }
        /// <summary>
        /// Effective deaths
        /// </summary>
        public int Death { get; set; }
        /// <summary>
        /// New confirmed
        /// </summary>
        public int NewConfirmed { get; set; }
        /// <summary>
        /// New death
        /// </summary>
        public int NewDeath { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Latitude
        /// </summary>
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"{DataProvider}{Area}{SubArea}{Admin2}{Date}{Latitude}{Longitude}";
        }
    }
}
