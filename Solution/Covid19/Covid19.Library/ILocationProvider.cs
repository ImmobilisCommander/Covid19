// <copyright file="ILocationProvider.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using System;

namespace Covid19.Library
{
    public interface ILocationProvider : IDisposable
    {
        void GetCoordinates(string countryRegion, string adminDistrict, string locality, out double latitude, out double longitude);
    }
}