using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Covid19.Library
{
    public class BingLocationProvider : IDisposable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BingLocationProvider));

        private readonly Dictionary<string, Tuple<double, double>> _coordinates = null;
        private readonly string _coordinatesFilePath = null;
        private readonly string _bingKey = null;
        private bool disposedValue = false;

        public BingLocationProvider(string bingKey, string pathToCoordinatesFile = null)
        {
            _bingKey = bingKey;
            _coordinatesFilePath = pathToCoordinatesFile;
            _coordinates = new Dictionary<string, Tuple<double, double>>();

            if (!string.IsNullOrEmpty(_coordinatesFilePath) && File.Exists(_coordinatesFilePath))
            {
                var tempCoords = JsonConvert.DeserializeObject<Dictionary<string, Tuple<double, double>>>(File.ReadAllText(_coordinatesFilePath));

                foreach (var item in tempCoords)
                {
                    if (!_coordinates.ContainsKey(item.Key.Trim()))
                    {
                        _coordinates.Add(item.Key.Trim(), item.Value);
                    }
                }
                logger.Debug($"Fichier de coordonnées \"{_coordinatesFilePath}\" lu. Nombre de coordonnées: {_coordinates.Count}");
            }
        }

        public void GetCoordinates(string countryRegion, string adminDistrict, string locality, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;
            var key = $"{countryRegion} {adminDistrict} {locality}".Trim();
            if (_coordinates.ContainsKey(key))
            {
                var coord = _coordinates[key];
                latitude = coord.Item1;
                longitude = coord.Item2;
            }
            else
            {
                using (var c = new WebClient())
                {
                    var result = c.DownloadString(new Uri($"http://dev.virtualearth.net/REST/v1/Locations?countryRegion={countryRegion}&adminDistrict={adminDistrict}&locality={locality}&key={_bingKey}"));
                    var temp = JsonConvert.DeserializeObject<LocationResult>(result);

                    if (temp != null)
                    {
                        latitude = Convert.ToDouble(temp.resourceSets?.FirstOrDefault()?.resources?.FirstOrDefault()?.Point?.Latitude);
                        longitude = Convert.ToDouble(temp.resourceSets?.FirstOrDefault()?.resources?.FirstOrDefault()?.Point?.Longitude);
                        _coordinates.Add(key, new Tuple<double, double>(latitude, longitude));

                        logger.Debug($"Bing returned \"{latitude}, {longitude}\" for \"{key}\"");
                    }
                    else
                    {
                        logger.Debug($"Bing returned null for \"{key}\"");
                    }
                }
            }
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés).
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                File.WriteAllText(_coordinatesFilePath, JsonConvert.SerializeObject(_coordinates));
                logger.Debug($"Coordonnées sauvegardées dans le fichier \"{_coordinatesFilePath}\"");

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~BingLocationProvider()
        // {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        public void Dispose()
        {
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region BING LOCATION RESPONSE
        internal class LocationResult
        {
            public List<resourceSet> resourceSets { get; set; }
        }

        internal class resourceSet
        {
            public List<resource> resources { get; set; }
        }

        internal class resource
        {
            public string Name { get; set; }

            public point Point { get; set; }
        }

        internal class point
        {
            public List<double> coordinates { get; set; }

            public double Latitude { get { return coordinates.FirstOrDefault(); } }
            public double Longitude { get { return coordinates.LastOrDefault(); } }
        }

        #endregion
    }
}
