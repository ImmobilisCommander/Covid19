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
    }

    public class DownloaderElement : ConfigurationElement
    {
        /// <summary>
        /// Whether download should be forced eventhough file already exists in repository
        /// </summary>
        [ConfigurationProperty("forceDownload")]
        public bool? ForceDownload { get { return (bool?)base["forceDownload"]; } }

        /// <summary>
        /// Identity name
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name { get { return (string)base["name"]; } }

        /// <summary>
        /// Identity email
        /// </summary>
        [ConfigurationProperty("email")]
        public string Email { get { return (string)base["email"]; } }

        /// <summary>
        /// Root path to the repository
        /// </summary>
        [ConfigurationProperty("repositoryPath")]
        public string RepositoryPath { get { return (string)base["repositoryPath"]; } }
    }
}
