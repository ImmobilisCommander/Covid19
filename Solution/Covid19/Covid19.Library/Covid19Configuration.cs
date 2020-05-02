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
        public FileElement Coordinates { get { return (FileElement)base["coordinates"]; } }

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
        /// John Hopkins downloader configuration
        /// </summary>
        [ConfigurationProperty("jhopkinsDownloader")]
        public DownloaderElement JohnHopkinsDownloader { get { return (DownloaderElement)base["jhopkinsDownloader"]; } }

        /// <summary>
        /// John Hopkins provider configuration
        /// </summary>
        [ConfigurationProperty("jhopkinsProvider")]
        public ProviderElement JohnHopkinsProvider { get { return (ProviderElement)base["jhopkinsProvider"]; } }
    }
}
