using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Covid19.Library
{
    /// <summary>
    /// Provide methods to use Bing location service
    /// </summary>
    public class BingLocationProvider : IDisposable
    {
        #region MEMBERS
        private static readonly ILog logger = LogManager.GetLogger(typeof(BingLocationProvider));

        private readonly Dictionary<string, Coordinates> _coordinates = null;
        private readonly string _coordinatesFilePath = null;
        private readonly string _bingKey = null;
        private bool _disposedValue = false; 
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bingKey">Bing Key to access service</param>
        /// <param name="pathToCoordinatesFile">Full path to coordinates files in which are stored previous requests to Bing service results</param>
        public BingLocationProvider(string bingKey, string pathToCoordinatesFile = null)
        {
            _bingKey = bingKey;
            _coordinatesFilePath = pathToCoordinatesFile;
            _coordinates = new Dictionary<string, Coordinates>();

            if (!string.IsNullOrEmpty(_coordinatesFilePath) && File.Exists(_coordinatesFilePath))
            {
                var temp = JsonConvert.DeserializeObject<List<Coordinates>>(File.ReadAllText(_coordinatesFilePath));
                _coordinates = temp.ToDictionary(x => x.LocationName);

                logger.Debug($"Coordinates file \"{_coordinatesFilePath}\" processed. Number of items: {_coordinates.Count}");
            }
        }

        // ~BingLocationProvider()
        // {
        //   Dispose(false);
        // }

        /// <summary>
        /// Provide coordinates from location names
        /// </summary>
        /// <param name="countryRegion">Country or region</param>
        /// <param name="adminDistrict">Administrative district</param>
        /// <param name="locality">Locality or town</param>
        /// <param name="latitude">Latitude of location</param>
        /// <param name="longitude">Longitude of location</param>
        public void GetCoordinates(string countryRegion, string adminDistrict, string locality, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;
            var key = $"{countryRegion} {adminDistrict} {locality}".GetKey();

            if (_coordinates.ContainsKey(key))
            {
                var coord = _coordinates[key];
                latitude = coord.Latitude;
                longitude = coord.Longitude;
            }
            else
            {
                using (var c = new WebClient())
                {
                    var result = c.DownloadString(new Uri($"http://dev.virtualearth.net/REST/v1/Locations?countryRegion={countryRegion}&adminDistrict={adminDistrict}&locality={locality}&key={_bingKey}"));
                    var temp = JsonConvert.DeserializeObject<LocationResult>(result);

                    if (temp != null)
                    {
                        latitude = Convert.ToDouble(temp.ResourceSets?.FirstOrDefault()?.Resources?.FirstOrDefault()?.Point?.Latitude);
                        longitude = Convert.ToDouble(temp.ResourceSets?.FirstOrDefault()?.Resources?.FirstOrDefault()?.Point?.Longitude);
                        _coordinates.Add(key, new Coordinates { LocationName = key, Latitude = latitude, Longitude = longitude });

                        logger.Debug($"Bing returned \"{latitude}, {longitude}\" for \"{key}\"");
                    }
                    else
                    {
                        logger.Debug($"Bing returned null for \"{key}\"");
                    }
                }
            }
        }

        /// <summary>
        /// Free memory by direct call to dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                File.WriteAllText(_coordinatesFilePath, JsonConvert.SerializeObject(_coordinates.OrderBy(x => x.Key).Select(x => x.Value), Formatting.Indented));
                logger.Debug($"Coordinates saved into following file: \"{_coordinatesFilePath}\"");

                _disposedValue = true;
            }
        }

        #region BING LOCATION RESPONSE
        internal class LocationResult
        {
            public List<ResourceSet> ResourceSets { get; set; }
        }

        internal class ResourceSet
        {
            public List<Resource> Resources { get; set; }
        }

        internal class Resource
        {
            public string Name { get; set; }

            public Point Point { get; set; }
        }

        internal class Point
        {
            public List<double> Coordinates { get; set; }

            public double Latitude { get { return Coordinates.FirstOrDefault(); } }
            public double Longitude { get { return Coordinates.LastOrDefault(); } }
        }

        #endregion
    }
}
