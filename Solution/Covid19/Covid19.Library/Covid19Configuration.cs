// <copyright file="Covid19Configuration.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using System.Configuration;

namespace Covid19.Library
{
    /// <summary>
    /// Configuration of application
    /// </summary>
    public class Covid19Configuration : ConfigurationSection
    {
        /// <summary>
        /// Configuration of coordinates file
        /// </summary>
        [ConfigurationProperty("coordinates")]
        public LocationProviderElement Coordinates { get { return (LocationProviderElement)base["coordinates"]; } }

        /// <summary>
        /// Configuration of final output file
        /// </summary>
        [ConfigurationProperty("mergedOutputFile")]
        public FileElement MergedOutputFile { get { return (FileElement)base["mergedOutputFile"]; } }

        /// <summary>
        /// ECDC downloader configuration
        /// </summary>
        [ConfigurationProperty("ecdcDownloader")]
        public DownloaderElement EcdcDownloader { get { return (DownloaderElement)base["ecdcDownloader"]; } }

        /// <summary>
        /// ECDC provider configuration
        /// </summary>
        [ConfigurationProperty("ecdcProvider")]
        public ProviderElement EcdcProvider { get { return (ProviderElement)base["ecdcProvider"]; } }

        /// <summary>
        /// Johns Hopkins downloader configuration
        /// </summary>
        [ConfigurationProperty("jhopkinsDownloader")]
        public DownloaderElement JohnsHopkinsDownloader { get { return (DownloaderElement)base["jhopkinsDownloader"]; } }

        /// <summary>
        /// Johns Hopkins provider configuration
        /// </summary>
        [ConfigurationProperty("jhopkinsProvider")]
        public ProviderElement JohnsHopkinsProvider { get { return (ProviderElement)base["jhopkinsProvider"]; } }
    }
}
