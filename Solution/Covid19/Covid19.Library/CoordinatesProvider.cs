// <copyright file="CoordinatesProvider.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using log4net;
using System;
using System.Collections.Generic;

namespace Covid19.Library
{
    /// <summary>
    /// Provider for geographic coordinates
    /// </summary>
    public class CoordinatesProvider
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CoordinatesProvider));
        private readonly string _coordinatesFilePath;
        private readonly string _locationProviderKey;
        private readonly string _providerName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="coordinatesFilePath">Path to the file that stores the coordinates</param>
        /// <param name="locationProviderKey">Bing key for web request</param>
        public CoordinatesProvider(string providerName, string coordinatesFilePath, string locationProviderKey)
        {
            this._providerName = providerName;
            this._coordinatesFilePath = coordinatesFilePath;
            this._locationProviderKey = locationProviderKey;
        }

        /// <summary>
        /// Set coordinates for all records that have there latitude set to 0
        /// </summary>
        /// <param name="data">Data to check and records to update</param>
        public void SetCoordinates(Dictionary<string, RawData> data)
        {
            using (var locationProvider = LocationProviderFactory.Create(_providerName, _locationProviderKey, _coordinatesFilePath))
            {
                double latitude;
                double longitude;

                foreach (var obj in data.Values)
                {
                    try
                    {
                        locationProvider.GetCoordinates(obj.Area, obj.SubArea, obj.Admin2, out latitude, out longitude);
                        obj.Latitude = latitude;
                        obj.Longitude = longitude;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"\"{obj}\": {ex.Message}", ex);
                    }
                }
            };
        }
    }
}
