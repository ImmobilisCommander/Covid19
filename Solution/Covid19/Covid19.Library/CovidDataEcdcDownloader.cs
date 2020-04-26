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
    public class CovidDataEcdcDownloader
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CovidDataEcdcDownloader));
        private readonly string downloadFolder;

        public CovidDataEcdcDownloader(string downloadFolder)
        {
            this.downloadFolder = downloadFolder;
        }

        /// <summary>
        /// Download data from ECDC. First data format was xls, then xlsx.
        /// Tries to download xls first if file does not exist then tries to download xlsx.
        /// </summary>
        public void DownloadFiles()
        {
            var rootUrl = "https://www.ecdc.europa.eu";

            var existingFiles = Directory.GetFiles(downloadFolder).ToList();

            var fromDate = new DateTime(2020, 3, 7);
            var nbDaysToNow = (DateTime.Now.AddDays(1) - fromDate).Days;
            using (var wc = new WebClientWithTimeout())
            {
                for (int i = 0; i < nbDaysToNow; i++)
                {
                    var fileName = $"COVID-19-geographic-disbtribution-worldwide-{fromDate.AddDays(i):yyyy-MM-dd}";
                    if (!existingFiles.Any(x => x.Contains(fileName)))
                    {
                        var url = $"{rootUrl}/sites/default/files/documents/{fileName}.xls";

                        try
                        {
                            wc.DownloadFile(url, Path.Combine(downloadFolder, $"{fileName}.xls"));
                            logger.Debug(url);
                        }
                        catch (WebException ex1)
                        {
                            if (ex1.Response != null)
                            {
                                logger.Error($"{(ex1.Response as HttpWebResponse).StatusCode}: {url}");
                            }
                            else
                            {
                                logger.Error($"{ex1.Message}: {url}");
                            }

                            try
                            {
                                url = $"{url}x";
                                wc.DownloadFile(url, Path.Combine(downloadFolder, $"{fileName}.xlsx"));
                            }
                            catch (WebException ex2)
                            {
                                if (ex2.Response != null)
                                {
                                    logger.Error($"{(ex2.Response as HttpWebResponse).StatusCode}: {url}");
                                }
                                else
                                {
                                    logger.Error($"{ex2.Message}: {url}");
                                }
                            }
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
}
