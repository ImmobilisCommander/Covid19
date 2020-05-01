using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Covid19.Library
{
    /// <summary>
    /// Provide methods to download data files from ECDC
    /// </summary>
    public class CovidDataEcdcDownloader
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CovidDataEcdcDownloader));
        private readonly string downloadFolder;
        private readonly bool? forceDownload;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="downloadFolder">Path to download repository</param>
        public CovidDataEcdcDownloader(string downloadFolder, bool? forceDownload)
        {
            this.downloadFolder = downloadFolder;
            this.forceDownload = forceDownload;
        }

        /// <summary>
        /// Download data from ECDC. First data format was xls, then xlsx.
        /// Tries to download xls first if file does not exist then tries to download xlsx.
        /// </summary>
        public void DownloadFiles()
        {
            var rootUrl = "https://www.ecdc.europa.eu";
            var existingFiles = Directory.GetFiles(downloadFolder).ToList();
            // Date of the first available file
            var fromDate = new DateTime(2020, 3, 7);
            // Calculate the number of days for each file to download
            var nbDaysToNow = (DateTime.Now.AddDays(1) - fromDate).Days;

            for (int i = 0; i < nbDaysToNow; i++)
            {
                var fileName = $"COVID-19-geographic-disbtribution-worldwide-{fromDate.AddDays(i):yyyy-MM-dd}";

                // If file has been already downloaded from a previous run don't dowload it again
                if ((forceDownload.HasValue && forceDownload.Value) || !existingFiles.Any(x => x.Contains(fileName)))
                {
                    // First files are in xls format
                    var url = $"{rootUrl}/sites/default/files/documents/{fileName}.xls";

                    var uri = new Uri(url);

                    if (!uri.DownloadTo(Path.Combine(downloadFolder, $"{fileName}.xls")))
                    {
                        logger.Debug($"Could not download \"{url}\"");

                        // Try download xlsx format file
                        url = $"{url}x";
                        uri = new Uri(url);
                        if (!uri.DownloadTo(Path.Combine(downloadFolder, $"{fileName}.xlsx")))
                        {
                            logger.Warn($"Could not download \"{url}\"");
                        }
                        else
                        {
                            logger.Info($"Downloaded \"{url}\"");
                        }
                    }
                    else
                    {
                        logger.Info($"Downloaded \"{url}\"");
                    }
                }
                else
                {
                    logger.Debug($"File \"{fileName}\" already exists");
                }
            }
        }
    }
}
