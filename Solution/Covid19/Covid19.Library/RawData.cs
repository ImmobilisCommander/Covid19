using System;
using System.Globalization;

namespace Covid19.Library
{
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

        public string DataProvider { get; set; }
        public int Id { get; set; }
        public string Area { get; set; }
        public string SubArea { get; set; }
        public string Admin2 { get; set; }
        public DateTime Date { get; set; }
        public int Confirmed { get; set; }
        public int Death { get; set; }
        public int NewConfirmed { get; set; }
        public int NewDeath { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"{DataProvider}{Area}{SubArea}{Admin2}{Date}{Latitude}{Longitude}";
        }
    }
}
