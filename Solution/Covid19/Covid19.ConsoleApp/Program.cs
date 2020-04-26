using Covid19.Library;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            logger.Info("Process Started");

            var ecdcDownloader = new CovidDataEcdcDownloader(@"E:\Git\ImmobilisCommander\Covid19\Data\ECDC");
            ecdcDownloader.DownloadFiles();

            var ecdcExtractor = new CovidDataEcdcExtractor(@"E:\Git\ImmobilisCommander\Covid19\Data\ECDC", @"E:\Git\ImmobilisCommander\Covid19\Data\Covid_ECDC.csv");
            ecdcExtractor.Extract();

            var johnHopkinsExtractor = new CovidDataJohnsHopkinsExtractor(@"E:\Git\COVID-19\csse_covid_19_data\csse_covid_19_daily_reports", @"E:\Git\ImmobilisCommander\Covid19\Data\Covid_JohnsHopkins.csv");
            johnHopkinsExtractor.Extract();

            var mergeFiles = new CovidDataMerge(@"E:\Git\ImmobilisCommander\Covid19\Data\Coordinates.json");
            mergeFiles.Merge(new string[] { @"E:\Git\ImmobilisCommander\Covid19\Data\Covid_ECDC.csv", @"E:\Git\ImmobilisCommander\Covid19\Data\Covid_JohnsHopkins.csv" }, @"E:\Git\ImmobilisCommander\Covid19\Data\covid_MergeData.csv");

            logger.Info("Process Ended");
        }
    }
}
