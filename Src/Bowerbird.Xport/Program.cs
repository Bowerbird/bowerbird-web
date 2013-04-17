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

namespace Bowerbird.Xport
{
    public class Program
    {
        static void Main(string[] args)
        {
            DataCrawler.Crawl();
        }
    }

    /// <summary>
    /// Hit the database for pages of objects
    /// </summary>
    internal class DataCrawler
    {
        private RavenQueryStatistics _stats;
        private int _totalRecrods = 0;
        private int _processedRecords = 0;
        private DumpFile _dumpFile;

        public static void Crawl()
        {
            new DataCrawler();
        }

        private DataCrawler()
        {
            _dumpFile = new DumpFile();

            CrawlTheData();
        }

        private void CrawlTheData()
        {
            var sightingsQuery = new SightingsQueryInput()
                                     {
                                         Page = 1,
                                         NeedsId = false,
                                         PageSize = 100
                                     };

            _dumpFile.SavePagedObjects(RunQuery(sightingsQuery));

            while (_processedRecords < _totalRecrods)
            {
                sightingsQuery.Page++;
                _dumpFile.SavePagedObjects(RunQuery(sightingsQuery));
            }
        }

        private PagedList<string> RunQuery(SightingsQueryInput sightingsQueryInput)
        {
            var documentStore = new DocumentStore
                                    {
                                        ConnectionStringName = ConfigSettings.Singleton().GetDatabaseUrl ( ) ,
                                        DefaultDatabase = ConfigSettings.Singleton ( ).GetDatabaseName ( )
                                    };

            using (var session = documentStore.OpenSession())
            {
                var query = session
                    .Advanced
                    .LuceneQuery<All_Contributions.Result, All_Contributions>()
                    .Statistics(out _stats)
                    .SelectFields<All_Contributions.Result>("GroupIds", "CreatedDateTime", "ParentContributionId",
                                                            "SubContributionId", "ParentContributionType",
                                                            "SubContributionType", "UserId", "Observation", "Record",
                                                            "Post", "User")
                    .WhereIn("ParentContributionType", new[] {"observation"})
                    .AndAlso()
                    .WhereEquals("SubContributionType", null);

                var result = query
                    .Skip(sightingsQueryInput.GetSkipIndex())
                    .Take(sightingsQueryInput.GetPageSize())
                    .ToList()
                    .Select(x => Transformer.MakeSighting(x.Contribution as Observation, x.User))
                    .ToPagedList(
                        sightingsQueryInput.GetPage(),
                        sightingsQueryInput.GetPageSize(),
                        _stats.TotalResults
                    );

                _totalRecrods = result.TotalResultCount;
                _processedRecords += result.PagedListItems.Count();

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
                "{0}/BowerbirdExport-{1}.txt",
                ConfigSettings.Singleton().GetEnvironmentRootPath(),
                DateTime.UtcNow.ToShortDateString()
                );
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
        public static string MakeSighting(Observation observation, User user)
        {
            StringBuilder str = new StringBuilder ( );

            var delimiter = ConfigSettings.Singleton ( ).GetDelimiter ( );

            var identification = observation
                .Identifications
                .OrderByDescending(y => y.Votes.Count())
                .ThenByDescending(y => y.CreatedOn)
                .FirstOrDefault();

            var mediaResource = observation.PrimaryMedia.MediaResource is ImageMediaResource ? observation.PrimaryMedia.MediaResource as ImageMediaResource: null;

            str.Append ( string.Format ( "{0}{1}" , ConfigSettings.Singleton ( ).GetUriToSite ( ) , observation.Id ) )
                .Append ( delimiter )
                .Append ( observation.Title )
                .Append ( delimiter )
                .Append ( observation.ObservedOn.ToShortDateString ( ) )
                .Append ( delimiter )
                .Append ( observation.Latitude )
                .Append ( delimiter )
                .Append ( observation.Longitude )
                .Append ( delimiter )
                .Append ( identification.Taxonomy )
                .Append ( delimiter )
                .Append ( user.Name )
                .Append ( delimiter )
                .Append ( mediaResource != null ? mediaResource.Image.Original.Uri : "No Image" )
                .Append ( delimiter )
                .Append ( mediaResource != null ? observation.PrimaryMedia.Licence : "No Image" );

            return str.ToString ( );
        }
    }
}