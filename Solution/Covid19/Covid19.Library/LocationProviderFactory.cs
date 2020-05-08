namespace Covid19.Library
{
    /// <summary>
    /// Factory for location providers
    /// </summary>
    internal class LocationProviderFactory
    {
        /// <summary>
        /// Creates a location provider depending on the provider's name
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="locationProviderKey">API Key</param>
        /// <param name="coordinatesFilePath">Full path to the location coordinates data</param>
        /// <returns></returns>
        public static ILocationProvider Create(string providerName, string locationProviderKey, string coordinatesFilePath)
        {
            ILocationProvider retour = null;
            switch (providerName.ToLowerInvariant())
            {
                case "bing":
                    retour = new BingLocationProvider(locationProviderKey, coordinatesFilePath);
                    break;
                case "google":
                default:
                    break;
            }
            return retour;
        }
    }
}
