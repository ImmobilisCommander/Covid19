using System.Configuration;

namespace Covid19.Library
{
    public class Covid19Configuration : ConfigurationSection
    {
        [ConfigurationProperty("coordinates")]
        public FileElement Coordinates { get { return (FileElement)base["coordinates"]; } }

        [ConfigurationProperty("mergedOutputFile")]
        public FileElement MergedOutputFile { get { return (FileElement)base["mergedOutputFile"]; } }

        [ConfigurationProperty("ecdc")]
        public ProviderElement Ecdc { get { return (ProviderElement)base["ecdc"]; } }

        [ConfigurationProperty("jhopkins")]
        public ProviderElement JohnHopkins { get { return (ProviderElement)base["jhopkins"]; } }
    }

    public class FileElement : ConfigurationElement
    {
        [ConfigurationProperty("path")]
        public string Path { get { return (string)base["path"]; } }
    }

    public class ProviderElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name { get { return (string)base["name"]; } }

        [ConfigurationProperty("repositoryFolder")]
        public string RepositoryFolder { get { return (string)base["repositoryFolder"]; } }

        [ConfigurationProperty("outputFile")]
        public string OutputFile { get { return (string)base["outputFile"]; } }

        [ConfigurationProperty("forceDownload")]
        public bool? ForceDownload { get { return (bool?)base["forceDownload"]; } }
    }
}
