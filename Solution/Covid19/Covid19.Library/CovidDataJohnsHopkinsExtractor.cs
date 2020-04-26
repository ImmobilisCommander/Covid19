using CsvHelper;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19.Library
{
    public class CovidDataJohnsHopkinsExtractor
    {
        #region Members
        private static readonly ILog logger = LogManager.GetLogger(typeof(CovidDataJohnsHopkinsExtractor));

        private static readonly CultureInfo _us = CultureInfo.GetCultureInfo("en-US");

        private static readonly string[] dateFormats = new string[]
        {
              // 2/1/20 19:43
              "M/d/yy HH:mm"
            // 2/5/20 1:43
            , "M/d/yy H:mm"
            , "MM/dd/yy HH:mm"
            , "MM/dd/yy HH:mm:ss"

            // 2/2/2020 2:13
            , "M/d/yyyy H:mm"
            , "M/d/yyyy HH:mm"
            , "MM/dd/yyyy HH:mm"

            // 2/3/2020 0:43:00
            , "M/d/yyyy H:mm:ss"
            , "M/d/yyyy HH:mm:ss"
            , "MM/dd/yyyy HH:mm:ss"

            // 2020-02-10 00:43:02
            , "yyyy-MM-dd HH:mm:ss"
            , "g"
            , "s"
            // 1/21/2020 10pm
            , "M/dd/yyyy tt"
            , "MM-dd-yyyy"
        };
        private readonly string repositoryFolder;
        private readonly string outputFile;

        public CovidDataJohnsHopkinsExtractor(string repositoryFolder, string outputFile)
        {
            this.repositoryFolder = repositoryFolder;
            this.outputFile = outputFile;
        }
        #endregion

        public void Extract()
        {
            var files = Directory.GetFiles(repositoryFolder, "*.csv", SearchOption.AllDirectories).ToList().OrderBy(x => x);

            var data = new Dictionary<string, RawData>();

            var datesErrors = new HashSet<string>();
            var missingFields = new HashSet<Tuple<string, string>>();

            var areaAlias = new string[] { "Country_Region", "Country/Region" };
            var subareaAlias = new string[] { "Province_State", "Province/State" };
            var latAlias = new string[] { "Latitude", "Lat" };
            var lngAlias = new string[] { "Longitude", "Long_" };

            foreach (var f in files)
            #region Extract data from files
            {
                int counter = 0;
                int addedLines = 0;
                using (var sr = new StreamReader(f))
                {
                    using (var csv = new CustomCsvReader(sr, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.MissingFieldFound = delegate (string[] tab, int count, ReadingContext ctxt)
                        {
                            var miss = new Tuple<string, string>(Path.GetFileName(f), string.Join(", ", tab));
                            if (!missingFields.Contains(miss))
                            {
                                missingFields.Add(miss);
                            }
                        };
                        csv.Read();
                        csv.ReadHeader();

                        var idxArea = csv.GetFieldIndex(areaAlias);
                        var idxSub = csv.GetFieldIndex(subareaAlias);
                        var idxAdmin2 = csv.GetFieldIndex("Admin2");
                        var idxConf = csv.GetFieldIndex("Confirmed");
                        var idxDeath = csv.GetFieldIndex("Deaths");
                        var idxLat = csv.GetFieldIndex(latAlias);
                        var idxLng = csv.GetFieldIndex(lngAlias);

                        while (csv.Read())
                        {
                            counter++;

                            try
                            {
                                var date = Path.GetFileNameWithoutExtension(f);
                                DateTime lastUpdate;

                                if (DateTime.TryParseExact(date, dateFormats, CultureInfo.GetCultureInfo("fr-fr"), DateTimeStyles.AdjustToUniversal, out lastUpdate))
                                {
                                    string area = csv.GetField(idxArea).Replace("Mainland China", "China").Replace("UK", "United Kingdom");
                                    string subarea = csv.GetField(idxSub);

                                    if (!string.IsNullOrEmpty(area))
                                    {
                                        var obj = new RawData();
                                        obj.DataProvider = "JohnHopkins";
                                        obj.Area = area;
                                        obj.SubArea = subarea;
                                        obj.Admin2 = csv.GetField(idxAdmin2)?.Replace("Unassigned", string.Empty);
                                        obj.Date = lastUpdate;
                                        obj.Confirmed = csv.GetFieldAsInt(idxConf);
                                        obj.Death = csv.GetFieldAsInt(idxDeath);
                                        obj.Latitude = csv.GetFieldAsDouble(idxLat, _us);
                                        obj.Longitude = csv.GetFieldAsDouble(idxLng, _us);

                                        if (!data.ContainsKey(obj.ToString()))
                                        {
                                            data.Add(obj.ToString(), obj);
                                            addedLines++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!datesErrors.Contains(date))
                                    {
                                        datesErrors.Add(string.Concat(csv.Context.Row.ToString().PadLeft(6, '0'), " \"", f, "\": ", date));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(string.Concat("\"", f, "\"", ": ", ex.Message));
                                logger.Debug(string.Join(" ; ", csv.Context.HeaderRecord));
                                logger.Debug(string.Join(" ; ", csv.Context.Record));
                            }
                        }
                    }
                }
                logger.Debug($"Traitement du fichier \"{Path.GetFileName(f)}\", lignes ajoutées/traitées: {addedLines}/{counter}");
            };
            #endregion

            if (missingFields.Count > 0)
            {
                logger.Warn($"Il y a {missingFields.GroupBy(x => x.Item1).Count()} fichiers dans lesquels une ou plusieurs colonnes n'ont pas été trouvées: \"{string.Join(", ", missingFields.GroupBy(x => x.Item2).Select(x => x.Key))}\"");
                if (logger.IsDebugEnabled)
                {
                    logger.Debug(string.Concat("Les champs suivants n'ont pas été trouvés:\n", string.Join("\n", missingFields.GroupBy(x => x.Item1).Select(x => $"{x.Key}: \"{string.Join("\", \"", x.Select(y => y.Item2))}\""))));
                }
            }

            if (datesErrors.Count > 0)
            {
                logger.Warn($"Il y a {datesErrors.Count} dates qui ne sont pas au bon format");
                if (logger.IsDebugEnabled)
                {
                    logger.Debug(string.Concat("Les dates suivantes ne sont pas au bon format:\n", string.Join("\n", datesErrors)));
                }
            }

            logger.Info(string.Concat("Found ", data.Count, " records"));

            using (var writer = new StreamWriter(outputFile))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.GetCultureInfo("fr-fr")))
                {
                    csv.WriteRecords(data.Select(x => x.Value));
                }
            }
        }
    }
}
