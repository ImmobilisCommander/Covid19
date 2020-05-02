using System.Configuration;

namespace Covid19.Library
{
    /// <summary>
    /// Data provider configuration
    /// </summary>
    public class ProviderElement : ConfigurationElement
    {
        /// <summary>
        /// Name of the provider
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name { get { return (string)base["name"]; } }

        /// <summary>
        /// Full path to the repository folder
        /// </summary>
        [ConfigurationProperty("repositoryFolder")]
        public string RepositoryFolder { get { return (string)base["repositoryFolder"]; } }

        /// <summary>
        /// Full path of the output file
        /// </summary>
        [ConfigurationProperty("outputFile")]
        public string OutputFile { get { return (string)base["outputFile"]; } }

        /// <summary>
        /// Full path of the output file
        /// </summary>
        [ConfigurationProperty("copyRepositoryFolder")]
        public string CopyRepositoryFolder { get { return (string)base["copyRepositoryFolder"]; } }
    }
}
