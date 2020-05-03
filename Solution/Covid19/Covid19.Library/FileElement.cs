// <copyright file="FileElement.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using System.Configuration;

namespace Covid19.Library
{
    /// <summary>
    /// File element configuration
    /// </summary>
    public class FileElement : ConfigurationElement
    {
        /// <summary>
        /// Full path to the file
        /// </summary>
        [ConfigurationProperty("path")]
        public string Path { get { return (string)base["path"]; } }
    }
}
