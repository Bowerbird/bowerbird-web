using System;
using System.Text;
using System.Configuration;
using System.Dynamic;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.ViewModels;
using Raven.Client;
using Raven.Client.Document;
using NLog;

namespace Bowerbird.Xport
{
    public class Program
    {
        static void Main(string[] args)
        {
            Logger _logger = LogManager.GetLogger("Bowerbird.Export");

            try
            {
                var dataCrawler = new DataCrawler();
                dataCrawler.CrawlTheData();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error exporting data", exception);
            }
        }
    }

    /// <summary>
    /// Hit the database for pages of objects
    /// </summary>
    public class DataCrawler
    {
        private int _totalRecrods = 0;
        private int _processedRecords = 0;
        private int _skippedResults;
        private DumpFile _dumpFile;
        private IDocumentStore _documentStore;
        private bool _anyMoreRecords = true;

        public DataCrawler()
        {
            _dumpFile = new DumpFile();
        }

        public void CrawlTheData()
        {
            _documentStore = new DocumentStore
            {
                Url = ConfigSettings.Singleton().GetDatabaseUrl(),
                DefaultDatabase = ConfigSettings.Singleton().GetDatabaseName()
            };

            _documentStore.Initialize();

            var sightingsQuery = new SightingsQueryInput()
                                     {
                                         Page = 1,
                                         NeedsId = false,
                                         PageSize = 100
                                     };

            _dumpFile.SavePagedObjects(RunQuery(sightingsQuery));

            while (_anyMoreRecords)
            {
                sightingsQuery.Page++;
                _dumpFile.SavePagedObjects(RunQuery(sightingsQuery));
            }
        }

        private PagedList<string> RunQuery(SightingsQueryInput sightingsQueryInput)
        {
            using (var session = _documentStore.OpenSession())
            {
                RavenQueryStatistics stats;
 
                var result = session.Query<Observation>()
                                .Statistics(out stats)
                                .Where(x => x.Identifications.Any() && x.Media.Any(y => y.MediaResource.MediaResourceType == "image"))
                                .Skip(sightingsQueryInput.GetSkipIndex())
                                .Take(sightingsQueryInput.GetPageSize())
                                .ToList()
                                .Select(Transformer.MakeSighting)
                                .ToPagedList(
                                    sightingsQueryInput.GetPage(),
                                    sightingsQueryInput.GetPageSize(),
                                    stats.TotalResults
                                );

                _anyMoreRecords = result.PagedListItems.Any();

                return result;
            }
        }
    }

    /// <summary>
    /// File creation and saving object
    /// </summary>
    internal class DumpFile
    {
        private readonly string _pathToFile;

        public DumpFile()
        {
            _pathToFile = string.Format(
                "{0}/data.txt",
                ConfigSettings.Singleton().GetEnvironmentRootPath()
                );

            if (File.Exists(_pathToFile))
            {
                File.Delete(_pathToFile);
            }
        }

        public void SavePagedObjects(PagedList<string> items)
        {
            using (StreamWriter writer = File.AppendText(_pathToFile))
            {
                foreach (var item in items.PagedListItems)
                {
                    writer.WriteLine(item);
                }
                writer.Flush();
            }
        }
    }

    /// <summary>
    /// App.Config property reader
    /// </summary>
    internal class ConfigSettings
    {
        private string _dumpFolder;
        private string _ravenInstanceUrl;
        private string _databaseName;
        private string _siteUrl;
        private string _delimiter;
        private static ConfigSettings _singleton;

        public static ConfigSettings Singleton ( )
        {
            if ( _singleton == null )
            {
                _singleton = new ConfigSettings ( );
            }

            return _singleton;
        }

        public string GetEnvironmentRootPath()
        {
            if ( string.IsNullOrEmpty ( _dumpFolder ) ) _dumpFolder = ConfigurationManager.AppSettings [ "dumpFolder" ];

            return _dumpFolder;
        }

        public string GetDatabaseUrl()
        {
            if ( string.IsNullOrEmpty ( _ravenInstanceUrl ) ) _ravenInstanceUrl = ConfigurationManager.AppSettings [ "ravenInstanceUrl" ];

            return _ravenInstanceUrl;
        }

        public string GetDatabaseName()
        {
            if ( string.IsNullOrEmpty ( _databaseName ) ) _databaseName = ConfigurationManager.AppSettings [ "databaseName" ];

            return _databaseName;
        }

        public string GetUriToSite ( )
        {
            if ( string.IsNullOrEmpty ( _siteUrl ) ) _siteUrl = ConfigurationManager.AppSettings [ "siteUrl" ];

            return _siteUrl;
        }

        public string GetDelimiter ( )
        {
            if ( string.IsNullOrEmpty ( _delimiter ) ) _delimiter = ConfigurationManager.AppSettings [ "delimiter" ];

            return _delimiter;
        }
    }

    /// <summary>
    /// Data format transformer
    /// </summary>
    internal class Transformer
    {
        public static string MakeSighting(Observation observation)
        {
            StringBuilder str = new StringBuilder ( );

            var delimiter = ConfigSettings.Singleton ( ).GetDelimiter ( );

            var identification = observation
                .Identifications
                .OrderByDescending(y => y.Votes.Count())
                .ThenByDescending(y => y.CreatedOn)
                .First();

            var mediaResource = observation.Media.First(x => x.MediaResource.MediaResourceType == "image").MediaResource as ImageMediaResource;

            var genus = string.Empty;
            var subgenus = string.Empty;

            var genusBits = identification.TryGetRankName("genus").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (genusBits.Any())
            {
                genus = genusBits[0];

                if (genusBits.Count() > 1)
                {
                    subgenus = genusBits[1];
                }
            }

            str.Append ( string.Format ( "{0}/{1}" , ConfigSettings.Singleton ( ).GetUriToSite ( ) , observation.Id ) )
                .Append ( delimiter )
                .Append ( observation.Title )
                .Append ( delimiter )
                .Append ( observation.ObservedOn.ToString("yyyy-MM-dd") )
                .Append ( delimiter )
                .Append ( observation.Latitude )
                .Append ( delimiter )
                .Append ( observation.Longitude )
                .Append ( delimiter )
                .Append ( identification.TryGetRankName("kingdom") )
                .Append ( delimiter )
                .Append(identification.TryGetRankName("phylum"))
                .Append(delimiter)
                .Append(identification.TryGetRankName("class"))
                .Append(delimiter)
                .Append(identification.TryGetRankName("order"))
                .Append(delimiter)
                .Append(identification.TryGetRankName("family"))
                .Append(delimiter)
                .Append(genus)
                .Append(delimiter)
                .Append(subgenus)
                .Append(delimiter)
                .Append(identification.TryGetRankName("species"))
                .Append(delimiter)
                .Append(identification.TryGetRankName("subspecies"))
                .Append(delimiter)
                .Append ( observation.User.Name )
                .Append ( delimiter )
                .Append ( mediaResource != null ? ConfigSettings.Singleton ( ).GetUriToSite ( ) + mediaResource.Image.Original.Uri : "No Image" )
                .Append ( delimiter )
                .Append ( mediaResource != null ? observation.PrimaryMedia.Licence : "No Image" );

            return str.ToString ( );
        }
    }
}