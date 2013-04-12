using System;
using System.Configuration;
using System.Dynamic;
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

        private PagedList<object> RunQuery(SightingsQueryInput sightingsQueryInput)
        {
            var config = new ConfigSettings();

            var documentStore = new DocumentStore
                                    {
                                        ConnectionStringName = config.GetDatabaseUrl(),
                                        DefaultDatabase = config.GetDatabaseName()
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
                    .WhereIn("ParentContributionType", new[] {"observation", "record"})
                    .AndAlso()
                    .WhereEquals("SubContributionType", null);

                var result = query
                    .Skip(sightingsQueryInput.GetSkipIndex())
                    .Take(sightingsQueryInput.GetPageSize())
                    .ToList()
                    .Select(x => Transformer.MakeSighting(x.Contribution as Sighting, x.User))
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
                new ConfigSettings().GetEnvironmentRootPath(),
                DateTime.UtcNow.ToShortDateString()
                );
        }

        public void SavePagedObjects(PagedList<object> items)
        {
            using (StreamWriter writer = File.AppendText(_pathToFile))
            {
                foreach (var item in items.PagedListItems)
                {
                    // Absolutely no idea what this is going to spew out....
                    writer.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
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
        public string GetEnvironmentRootPath()
        {
            return ConfigurationManager.AppSettings["dumpFolder"];
        }

        public string GetDatabaseUrl()
        {
            return ConfigurationManager.AppSettings["ravenInstanceUrl"];
        }

        public string GetDatabaseName()
        {
            return ConfigurationManager.AppSettings["databaseName"];
        }
    }

    /// <summary>
    /// Data format transformer
    /// </summary>
    internal class Transformer
    {
        public static object MakeSighting(Sighting sighting, User user)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sighting.Id;
            viewModel.CreatedOn = sighting.CreatedOn;
            viewModel.ObservedOn = sighting.ObservedOn;
            viewModel.Latitude = sighting.Latitude;
            viewModel.Longitude = sighting.Longitude;
            viewModel.Category = sighting.Category;
            viewModel.AnonymiseLocation = sighting.AnonymiseLocation;
            viewModel.User = MakeUser(user);
            viewModel.ObservedOnDescription = sighting.ObservedOn.ToString("d MMM yyyy");
            viewModel.CreatedOnDescription = sighting.CreatedOn.ToString("d MMM yyyy");
            viewModel.NoteCount = sighting.Notes.Count();
            viewModel.IdentificationCount = sighting.Identifications.Count();
            viewModel.FavouritesCount = sighting.Groups.Count(x => x.Group.GroupType == "favourites");
            viewModel.TotalVoteScore = sighting.Votes.Sum(x => x.Score);

            if (sighting is Observation)
            {
                var observation = sighting as Observation;

                viewModel.Title = observation.Title;
                viewModel.Address = observation.Address;
                viewModel.Media = observation.Media.Select(MakeObservationMedia);
                viewModel.PrimaryMedia = MakeObservationMedia(observation.PrimaryMedia);
                viewModel.MediaCount = observation.Media.Count();
                viewModel.ShowMediaThumbnails = observation.Media.Count() > 1;
            }

            return viewModel;
        }

        private static object MakeUser(User user)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = user.Id;
            viewModel.Name = user.Name;

            return viewModel;
        }

        private static object MakeObservationMedia(ObservationMedia observationMedia)
        {
            return new
            {
                observationMedia.Description,
                observationMedia.IsPrimaryMedia,
                observationMedia.Licence,
                MediaResource = MakeMediaResource(observationMedia.MediaResource)
            };
        }

        private static object MakeMediaResource(MediaResource mediaResource)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = mediaResource.Id;
            viewModel.MediaResourceType = mediaResource.MediaResourceType;
            viewModel.UploadedOn = mediaResource.UploadedOn;

            viewModel.User = new
            {
                mediaResource.CreatedByUser.Name
            };

            if (mediaResource is ImageMediaResource)
            {
                var imageMediaResource = mediaResource as ImageMediaResource;
                viewModel.Image = new ExpandoObject();

                if (imageMediaResource.Image.Original != null)
                {
                    viewModel.Image.Original = new
                    {
                        imageMediaResource.Image.Original.Uri
                    };
                }
            }
            if (mediaResource is VideoMediaResource)
            {
                var videoMediaResource = mediaResource as VideoMediaResource;
                viewModel.Video = new ExpandoObject();

                if (videoMediaResource.Video.Original != null)
                {
                    viewModel.Video.Original = new
                    {
                        videoMediaResource.Video.Original.Uri
                    };
                }
            }
            if (mediaResource is AudioMediaResource)
            {
                var audioMediaResource = mediaResource as AudioMediaResource;
                viewModel.Audio = new ExpandoObject();

                if (audioMediaResource.Audio.Original != null)
                {
                    viewModel.Audio.Original = new
                    {
                        audioMediaResource.Audio.Original.MimeType
                    };
                }
            }

            return viewModel;
        }
    }
}