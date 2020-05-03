// <copyright file="CovidDataJohnsHopkinsExtractor.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using CsvHelper;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Covid19.Library
{
    /// <summary>
    /// Data extractor for Johns Hopkins University data files
    /// </summary>
    public class CovidDataJohnsHopkinsExtractor
    {
        #region Members
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CovidDataJohnsHopkinsExtractor));
        private static readonly CultureInfo _us = CultureInfo.GetCultureInfo("en-US");
        private static readonly string[] _dateFormats = new string[]
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

        private readonly string _repositoryFolder;
        private readonly string _outputFile;
        private readonly string _copyRepositoryFolder;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositoryFolder">Path to the directory where the data files from Johns Hopkins are stored</param>
        /// <param name="outputFile">Path to the output CSV file</param>
        public CovidDataJohnsHopkinsExtractor(string repositoryFolder, string outputFile, string copyRepositoryFolder)
        {
            this._repositoryFolder = repositoryFolder;
            this._outputFile = outputFile;
            this._copyRepositoryFolder = copyRepositoryFolder;
        }

        /// <summary>
        /// Method that extract data and save them a normalized CSV file
        /// </summary>
        public Dictionary<string, RawData> Extract()
        {
            var data = new Dictionary<string, RawData>();
            var datesErrors = new HashSet<string>();
            var missingFields = new HashSet<Tuple<string, string>>();

            // Setting columns names aliases
            var areaAlias = new string[] { "Country_Region", "Country/Region" };
            var subareaAlias = new string[] { "Province_State", "Province/State" };
            var latAlias = new string[] { "Latitude", "Lat" };
            var lngAlias = new string[] { "Longitude", "Long_" };

            var files = Directory.GetFiles(_repositoryFolder, "*.csv", SearchOption.AllDirectories).ToList().OrderBy(x => x);

            foreach (var f in files)
            #region EXTRACT DATA FROM FILES
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

                        #region FETCH COLUMNS INDEX
                        var idxArea = csv.GetFieldIndex(areaAlias);
                        var idxSub = csv.GetFieldIndex(subareaAlias);
                        var idxAdmin2 = csv.GetFieldIndex("Admin2");
                        var idxConf = csv.GetFieldIndex("Confirmed");
                        var idxDeath = csv.GetFieldIndex("Deaths");
                        var idxLat = csv.GetFieldIndex(latAlias);
                        var idxLng = csv.GetFieldIndex(lngAlias); 
                        #endregion

                        while (csv.Read())
                        {
                            #region PROCESSING A LINE
                            counter++;

                            try
                            {
                                var date = Path.GetFileNameWithoutExtension(f);

                                if (DateTime.TryParseExact(date, _dateFormats, CultureInfo.GetCultureInfo("fr-fr"), DateTimeStyles.AdjustToUniversal, out DateTime lastUpdate))
                                {
                                    string area = csv.GetField(idxArea).Replace("Mainland China", "China").Replace("UK", "United Kingdom");
                                    string subarea = csv.GetField(idxSub);

                                    if (!string.IsNullOrEmpty(area))
                                    {
                                        var obj = new RawData();
                                        obj.DataProvider = "JohnsHopkins";
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
                                _logger.Error(string.Concat("\"", f, "\"", ": ", ex.Message));
                                _logger.Debug(string.Join(" ; ", csv.Context.HeaderRecord));
                                _logger.Debug(string.Join(" ; ", csv.Context.Record));
                                throw ex;
                            }
                            #endregion
                        }
                    }
                }

                var copyFile = Path.Combine(_copyRepositoryFolder, Path.GetFileName(f));
                if (!File.Exists(copyFile))
                {
                    // Make a copy
                    File.Copy(f, copyFile);
                }

                _logger.Debug($"Processing file \"{Path.GetFileName(f)}\", number of lines added/processed: {addedLines}/{counter}");
            };
            #endregion

            #region LOGGING ERRORS
            if (missingFields.Count > 0)
            {
                _logger.Warn($"There are {missingFields.GroupBy(x => x.Item1).Count()} files in which some columns could not be found: \"{string.Join(", ", missingFields.GroupBy(x => x.Item2).Select(x => x.Key))}\"");
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug(string.Concat("Following fields could not be found:\n", string.Join("\n", missingFields.GroupBy(x => x.Item1).Select(x => $"{x.Key}: \"{string.Join("\", \"", x.Select(y => y.Item2))}\""))));
                }
            }

            if (datesErrors.Count > 0)
            {
                _logger.Warn($"There are {datesErrors.Count} dates not correctly formated");
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug(string.Concat("Following dates are not in correct format:\n", string.Join("\n", datesErrors)));
                }
            } 
            #endregion

            _logger.Info(string.Concat("Found ", data.Count, " records"));

            // Writing output file
            using (var writer = new StreamWriter(_outputFile))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.GetCultureInfo("fr-fr")))
                {
                    csv.WriteRecords(data.Select(x => x.Value));
                }
            }

            return data;
        }
    }
}
