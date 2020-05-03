// <copyright file="CovidDataEcdcDownloader.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using log4net;
using System;
using System.IO;
using System.Linq;

namespace Covid19.Library
{
    /// <summary>
    /// Provide methods to download data files from ECDC
    /// </summary>
    public class CovidDataEcdcDownloader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CovidDataEcdcDownloader));
        private readonly string _downloadFolder;
        private readonly bool? _forceDownload;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="downloadFolder">Path to download repository</param>
        public CovidDataEcdcDownloader(string downloadFolder, bool? forceDownload)
        {
            this._downloadFolder = downloadFolder;
            this._forceDownload = forceDownload;
        }

        /// <summary>
        /// Download data from ECDC. First data format was xls, then xlsx.
        /// Tries to download xls first if file does not exist then tries to download xlsx.
        /// </summary>
        public void DownloadFiles()
        {
            var rootUrl = "https://www.ecdc.europa.eu";
            var existingFiles = Directory.GetFiles(_downloadFolder).ToList();
            // Date of the first available file
            var fromDate = new DateTime(2020, 3, 7);
            // Calculate the number of days for each file to download
            var nbDaysToNow = (DateTime.Now.AddDays(1) - fromDate).Days;

            for (int i = 0; i < nbDaysToNow; i++)
            {
                var fileName = $"COVID-19-geographic-disbtribution-worldwide-{fromDate.AddDays(i):yyyy-MM-dd}";

                // If file has been already downloaded from a previous run don't dowload it again
                if ((_forceDownload.HasValue && _forceDownload.Value) || !existingFiles.Any(x => x.Contains(fileName)))
                {
                    // First files are in xls format
                    var url = $"{rootUrl}/sites/default/files/documents/{fileName}.xls";

                    var uri = new Uri(url);

                    if (!uri.DownloadTo(Path.Combine(_downloadFolder, $"{fileName}.xls")))
                    {
                        _logger.Debug($"Could not download \"{url}\"");

                        // Try download xlsx format file
                        url = $"{url}x";
                        uri = new Uri(url);
                        if (!uri.DownloadTo(Path.Combine(_downloadFolder, $"{fileName}.xlsx")))
                        {
                            _logger.Warn($"Could not download \"{url}\"");
                        }
                        else
                        {
                            _logger.Info($"Downloaded \"{url}\"");
                        }
                    }
                    else
                    {
                        _logger.Info($"Downloaded \"{url}\"");
                    }
                }
                else
                {
                    _logger.Debug($"File \"{fileName}\" already exists");
                }
            }
        }
    }
}
