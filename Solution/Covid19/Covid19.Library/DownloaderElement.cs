// <copyright file="DownloaderElement.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using System.Configuration;

namespace Covid19.Library
{
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
