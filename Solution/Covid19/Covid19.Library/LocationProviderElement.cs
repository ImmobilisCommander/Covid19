// <copyright file="LocationProviderElement.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using System.Configuration;

namespace Covid19.Library
{
    public class LocationProviderElement : ConfigurationElement
    {
        /// <summary>
        /// Name of the provider
        /// </summary>
        [ConfigurationProperty("providerName")]
        public string ProviderName { get { return (string)base["providerName"]; } }

        /// <summary>
        /// Configuration of data file
        /// </summary>
        [ConfigurationProperty("dataFilePath")]
        public string DataFilePath { get { return (string)base["dataFilePath"]; } }
    }
}
