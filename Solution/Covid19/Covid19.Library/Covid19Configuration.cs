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
        /// ECDC provider configuration
        /// </summary>
        [ConfigurationProperty("ecdc")]
        public ProviderElement Ecdc { get { return (ProviderElement)base["ecdc"]; } }

        /// <summary>
        /// John Hopkins provider configuration
        /// </summary>
        [ConfigurationProperty("jhopkins")]
        public ProviderElement JohnHopkins { get { return (ProviderElement)base["jhopkins"]; } }
    }
}
