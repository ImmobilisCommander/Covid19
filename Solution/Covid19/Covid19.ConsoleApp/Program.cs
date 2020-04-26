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

            var ecdcDownloader = new CovidDataEcdcDownloader(ConfigurationManager.AppSettings["EcdcFolder"]);
            ecdcDownloader.DownloadFiles();

            var ecdcExtractor = new CovidDataEcdcExtractor(ConfigurationManager.AppSettings["EcdcFolder"], ConfigurationManager.AppSettings["EcdcOutputFile"]);
            ecdcExtractor.Extract();

            var johnHopkinsExtractor = new CovidDataJohnsHopkinsExtractor(ConfigurationManager.AppSettings["JHophinsFolder"], ConfigurationManager.AppSettings["JHophinsOutputFile"]);
            johnHopkinsExtractor.Extract();

            var mergeFiles = new CovidDataMerge(ConfigurationManager.AppSettings["Coordinates"], ConfigurationManager.AppSettings["bingKey"]);
            mergeFiles.Merge(new string[] { ConfigurationManager.AppSettings["EcdcOutputFile"], ConfigurationManager.AppSettings["JHophinsOutputFile"] }, ConfigurationManager.AppSettings["MergedOutputFile"]);

            logger.Info("Process Ended");
        }
    }
}
