// <copyright file="CovidDataJohnsHopkinsDownloader.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using LibGit2Sharp;
using log4net;
using System;

namespace Covid19.Library
{
    public class CovidDataJohnsHopkinsDownloader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CovidDataJohnsHopkinsDownloader));
        private readonly string repositoryPath;
        private readonly string name;
        private readonly string email;

        public CovidDataJohnsHopkinsDownloader(string repositoryPath, string name, string email)
        {
            this.repositoryPath = repositoryPath;
            this.name = name;
            this.email = email;
        }

        public void DownloadFiles()
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    // Credential information to fetch
                    var options = new PullOptions
                    {
                        FetchOptions = new FetchOptions()
                    };

                    // User information to create a merge commit
                    var signature = new Signature(new Identity(name, email), DateTimeOffset.Now);

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
