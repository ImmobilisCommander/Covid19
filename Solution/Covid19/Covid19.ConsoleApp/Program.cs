﻿using Covid19.Library;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            logger.Info("********************************************");
            logger.Info("Process Started");

            var sw = Stopwatch.StartNew();

            try
            {
                var config = (Covid19Configuration)ConfigurationManager.GetSection("covid");
                var bingKey = ConfigurationManager.AppSettings["bingKey"];

                var ecdcDownloader = new CovidDataEcdcDownloader(config.Ecdc.RepositoryFolder);
                ecdcDownloader.DownloadFiles();

                var ecdcExtractor = new CovidDataEcdcExtractor(config.Ecdc.RepositoryFolder, config.Ecdc.OutputFile);
                var data = ecdcExtractor.Extract();

                var johnHopkinsExtractor = new CovidDataJohnsHopkinsExtractor(config.JohnHopkins.RepositoryFolder, config.JohnHopkins.OutputFile);
                var temp = johnHopkinsExtractor.Extract();
                var result = data.Concat(temp).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());

                var mergeFiles = new CoordinatesProvider(config.Coordinates.Path, bingKey);
                mergeFiles.SetCoordinates(result);

                File.WriteAllText(config.MergedOutputFile.Path, result.Values.OrderBy(x => x.DataProvider).ThenBy(x => x.Area).ThenBy(x => x.SubArea).ThenBy(x => x.Admin2).ThenBy(x => x.Date).ToCsv());
            }
            catch (Exception e)
            {
                logger.Error(e.Message, e);
            }

            sw.Stop();

            logger.Info($"Process Ended. Duration {sw.Elapsed}.");
        }
    }
}
