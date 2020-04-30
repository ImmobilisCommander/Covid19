using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19.Library
{
    public class CoordinatesProvider
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CoordinatesProvider));
        private readonly string coordinatesFilePath;
        private readonly string bingKey;

        public CoordinatesProvider(string coordinatesFilePath, string bingKey)
        {
            this.coordinatesFilePath = coordinatesFilePath;
            this.bingKey = bingKey;
        }

        public void SetCoordinates(Dictionary<string, RawData> data)
        {
            if (data.Values.Any(x => x.Longitude == 0))
            {
                using (var locationProvider = new BingLocationProvider(bingKey, coordinatesFilePath))
                {
                    double latitude;
                    double longitude;

                    foreach (var obj in data.Values.Where(x => x.Longitude == 0))
                    {
                        try
                        {
                            locationProvider.GetCoordinates(obj.Area, obj.SubArea, obj.Admin2, out latitude, out longitude);
                            obj.Latitude = latitude;
                            obj.Longitude = longitude;
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"\"{obj}\": {ex.Message}", ex);
                        }
                    }
                };
            }
        }
    }
}
