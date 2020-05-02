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
                                // Data is incremental, must find the value of the previous day and sum it with current day
                                var temp = data.Values.Where(x => x.Area == obj.Area).FirstOrDefault(x => x.Date == obj.Date.AddDays(-1));
                                if (temp != null)
                                {
                                    obj.Confirmed += temp.Confirmed;
                                    obj.Death += temp.Death;
                                }
                                data.Add(obj.ToString(), obj);
                            }
                        }
                    }
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
