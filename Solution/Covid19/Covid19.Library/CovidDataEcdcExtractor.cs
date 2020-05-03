// <copyright file="CovidDataEcdcExtractor.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using CsvHelper;
using ExcelDataReader;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Covid19.Library
{
    /// <summary>
    /// 
    /// </summary>
    public class CovidDataEcdcExtractor
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CovidDataEcdcExtractor));
        private readonly string _repositoyFolder;
        private readonly string _outputFile;

        public CovidDataEcdcExtractor(string repositoyFolder, string outputFile)
        {
            this._repositoyFolder = repositoyFolder;
            this._outputFile = outputFile;
        }

        public Dictionary<string, RawData> Extract()
        {
            var f = Directory.GetFiles(_repositoyFolder).ToList().OrderBy(x => x).LastOrDefault();

            var data = new Dictionary<string, RawData>();

            try
            {
                _logger.Debug($"Reading file: \"{f}\"");
                using (var sr = File.OpenRead(f))
                {
                    using (var xl = ExcelReaderFactory.CreateReader(sr, null))
                    {
                        var conf = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };

                        var dataSet = xl.AsDataSet(conf);
                        var dataTable = dataSet.Tables[0];
                        var view = new DataView(dataTable)
                        {
                            Sort = "DateRep ASC"
                        };

                        var country = dataTable.Columns.IndexOf("CountryExp");

                        for (int i = 0; i < view.Count; i++)
                        {
                            var row = view[i];
                            var obj = new RawData() { DataProvider = "ECDC" };

                            if (country == -1)
                            {
                                obj.Area = row.TryGetValue("countriesAndTerritories");
                                obj.Date = new DateTime(Convert.ToInt32(row.TryGetValue("Year")), Convert.ToInt32(row.TryGetValue("Month")), Convert.ToInt32(row.TryGetValue("Day")));
                                obj.Confirmed = Convert.ToInt32(row.TryGetValue("Cases"));
                                obj.Death = Convert.ToInt32(row.TryGetValue("Deaths"));
                            }
                            else
                            {
                                obj.Area = row.TryGetValue("CountryExp");
                                obj.Date = Convert.ToDateTime(row.TryGetValue("DateRep"));
                            }

                            if (!data.ContainsKey(obj.ToString()))
                            {
                                data.Add(obj.ToString(), obj);
                            }
                        }
                    }
                }

                foreach (var area in data.Values.GroupBy(x => x.Area).OrderBy(x => x.Key))
                {
                    _logger.Debug($"Processing {area.Key}");

                    var missingData = new List<RawData>();
                    var mindate = area.Min(x => x.Date);
                    var maxdate = area.Max(x => x.Date);
                    var nbDays = (maxdate - mindate).Days;

                    RawData previous = null;
                    RawData current = null;

                    // For some reason some days have no data. Must create missing day.
                    for (int i = 0; i <= nbDays; i++)
                    {
                        current = area.FirstOrDefault(x => x.Date == mindate.AddDays(i));
                        if (current == null)
                        {
                            current = new RawData { DataProvider = "ECDC", Area = area.Key, Date = mindate.AddDays(i) };
                        }

                        // Don't take the first day as previous day does not exist
                        if (i > 0)
                        {
                            // data is incremental, take previous day
                            previous = area.FirstOrDefault(x => x.Date == current.Date.AddDays(-1));

                            // If previous was missing then added to main data source, main source is not refreshed. Keep missing data in a list aside
                            if (previous == null)
                            {
                                previous = missingData.FirstOrDefault(x => x.Date == current.Date.AddDays(-1));
                            }

                            current.Confirmed += previous.Confirmed;
                            current.Death += previous.Death;
                        }

                        if (!data.ContainsKey(current.ToString()))
                        {
                            missingData.Add(current);
                            data.Add(current.ToString(), current);
                        }
                    }

                    _logger.Debug($"Missing day created for: {string.Join("\n", missingData.Select(x => x.ToDisplayName()))}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            _logger.Info(string.Concat("Found ", data.Count, " records"));

            using (var writer = new StreamWriter(this._outputFile))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.GetCultureInfo("fr-fr")))
                {
                    csv.WriteRecords(data.Values.OrderBy(x => x.Area).ThenBy(x => x.Date));
                }
            }

            return data;
        }
    }
}
