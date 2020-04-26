using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19.Library
{
    public class CovidDataMerge
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CovidDataMerge));
        private readonly string coordinatesFilePath;

        public CovidDataMerge(string coordinatesFilePath)
        {
            this.coordinatesFilePath = coordinatesFilePath;
        }

        public void Merge(string[] files, string outputFile)
        {
            var datas = new ConcurrentBag<RawData>();
            var tasks = new List<Task>();

            foreach (var file in files)
            {
                using (var sr = new StreamReader(file))
                {
                    sr.ReadLine();
                    var counter = 0;
                    while (!sr.EndOfStream)
                    {
                        counter++;
                        var line = sr.ReadLine();
                        var task = Task.Factory.StartNew(() =>
                        {
                            datas.Add(new RawData(line));
                        });
                        tasks.Add(task);
                    }
                    logger.Info($"Found {counter} records in file \"{file}\"");
                }
            }

            Task.WaitAll(tasks.ToArray());

            logger.Debug($"Données reconsituées, {datas.Count} éléments");

            if (datas.Where(x => x.Longitude == 0).Count() > 0)
            {
                using (var locationProvider = new BingLocationProvider("AvYkxtYFYYkj88wdumo2eSv3Qu7Pt1VGHZq8LTCiZazP8LKV7mUzLvFu145QNuyO", coordinatesFilePath))
                {
                    double latitude;
                    double longitude;

                    foreach (var obj in datas.Where(x => x.Longitude == 0))
                    {
                        try
                        {
                            locationProvider.GetCoordinates(obj.Area, obj.SubArea, obj.Admin2, out latitude, out longitude);
                            obj.Latitude = latitude;
                            obj.Longitude = longitude;
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"\"{obj}\": {ex.Message}", ex);
                        }
                    }
                };
            }

            File.WriteAllText(outputFile, datas.OrderBy(x => x.DataProvider).ThenBy(x => x.Area).ThenBy(x => x.SubArea).ThenBy(x => x.Admin2).ThenBy(x => x.Date).ToCsv());
        }
    }
}
