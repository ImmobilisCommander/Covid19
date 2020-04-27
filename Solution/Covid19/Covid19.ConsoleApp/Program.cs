using Covid19.Library;
using log4net;
using System;
using System.Configuration;

namespace Covid19.ConsoleApp
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        static Program()
        {
            Console.Title = "Covid19";
            Console.WindowWidth = 200;

            log4net.Config.XmlConfigurator.Configure();
        }

        static void Main(string[] args)
        {
            logger.Info("\nProcess Started");

            try
            {
                var ecdcFolder = ConfigurationManager.AppSettings["EcdcFolder"];
                var ecdcOutputFile = ConfigurationManager.AppSettings["EcdcOutputFile"];
                var jHopkinsFolder = ConfigurationManager.AppSettings["JHophinsFolder"];
                var jHopkinsOutputFile = ConfigurationManager.AppSettings["JHophinsOutputFile"];
                var coordinatesFile = ConfigurationManager.AppSettings["Coordinates"];
                var bingKey = ConfigurationManager.AppSettings["bingKey"];
                var mergedOutputFile = ConfigurationManager.AppSettings["MergedOutputFile"];

                var ecdcDownloader = new CovidDataEcdcDownloader(ecdcFolder);
                ecdcDownloader.DownloadFiles();

                var ecdcExtractor = new CovidDataEcdcExtractor(ecdcFolder, ecdcOutputFile);
                ecdcExtractor.Extract();

                var johnHopkinsExtractor = new CovidDataJohnsHopkinsExtractor(jHopkinsFolder, jHopkinsOutputFile);
                johnHopkinsExtractor.Extract();

                var mergeFiles = new CovidDataMerge(coordinatesFile, bingKey);
                mergeFiles.Merge(new string[] { ecdcOutputFile, jHopkinsFolder }, mergedOutputFile);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            logger.Info("Process Ended");
        }
    }
}
