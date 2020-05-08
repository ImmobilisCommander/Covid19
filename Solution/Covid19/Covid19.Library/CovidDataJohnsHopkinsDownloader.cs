// <copyright file="CovidDataJohnsHopkinsDownloader.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using LibGit2Sharp;
using log4net;
using System;

namespace Covid19.Library
{
    /// <summary>
    /// File downloader for Johns Hopkins data files
    /// </summary>
    public class CovidDataJohnsHopkinsDownloader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CovidDataJohnsHopkinsDownloader));
        private readonly string _repositoryPath;
        private readonly string _name;
        private readonly string _email;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositoryPath">Full path to file repository</param>
        /// <param name="name">Name to use for Git identity</param>
        /// <param name="email">Email to use for Git identity</param>
        public CovidDataJohnsHopkinsDownloader(string repositoryPath, string name, string email)
        {
            this._repositoryPath = repositoryPath;
            this._name = name;
            this._email = email;
        }

        /// <summary>
        /// Download data from Johns Hopkins Git repository
        /// </summary>
        public void DownloadFiles()
        {
            try
            {
                using (var repo = new Repository(_repositoryPath))
                {
                    // Credential information to fetch
                    var options = new PullOptions
                    {
                        FetchOptions = new FetchOptions()
                    };

                    // User information to create a merge commit
                    var signature = new Signature(new Identity(_name, _email), DateTimeOffset.Now);

                    // Pull
                    var result = Commands.Pull(repo, signature, options);
                    _logger.Info($"Pull result: {result.Status}");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }
    }
}
