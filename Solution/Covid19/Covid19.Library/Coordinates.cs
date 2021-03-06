﻿// <copyright file="Coordinates.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

namespace Covid19.Library
{
    /// <summary>
    /// DTO for coordinates
    /// </summary>
    public class Coordinates
    {
        /// <summary>
        /// Name of the location. Used as key
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{LocationName}: {Latitude}, {Longitude}";
        }
    }
}
