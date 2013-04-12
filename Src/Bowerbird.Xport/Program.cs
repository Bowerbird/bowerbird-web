using System;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
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
            var dataCrawler = new DataCrawler();
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
                    .Select(
                        x => Transformer.MakeSighting(x.Contribution as Sighting, x.User, x.Groups, null))
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
    /// Data format transformer
    /// </summary>
    internal class Transformer
    {
        public static object MakeSighting(Sighting sighting, User user, IEnumerable<Group> projects, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sighting.Id;
            viewModel.CreatedOn = sighting.CreatedOn;
            viewModel.ObservedOn = sighting.ObservedOn;
            viewModel.Latitude = sighting.Latitude;
            viewModel.Longitude = sighting.Longitude;
            viewModel.Category = sighting.Category;
            viewModel.AnonymiseLocation = sighting.AnonymiseLocation;
            viewModel.Projects = projects.Where(x => x.GroupType == "project").Select(y => MakeGroup(y, authenticatedUser));
            viewModel.User = MakeUser(user, authenticatedUser);
            viewModel.ObservedOnDescription = sighting.ObservedOn.ToString("d MMM yyyy");
            viewModel.CreatedOnDescription = sighting.CreatedOn.ToString("d MMM yyyy");
            viewModel.CommentCount = sighting.Discussion.CommentCount;
            viewModel.ProjectCount = projects.Count(x => x.GroupType == "project");
            viewModel.NoteCount = sighting.Notes.Count();
            viewModel.IdentificationCount = sighting.Identifications.Count();
            viewModel.FavouritesCount = sighting.Groups.Count(x => x.Group.GroupType == "favourites");
            viewModel.TotalVoteScore = sighting.Votes.Sum(x => x.Score);

            // Current user-specific properties
            if (authenticatedUser != null)
            {
                var userId = authenticatedUser.Id;
                var favouritesId = authenticatedUser.Memberships.Single(x => x.Group.GroupType == "favourites").Group.Id;

                viewModel.UserVoteScore = sighting.Votes.Any(x => x.User.Id == userId) ? sighting.Votes.Single(x => x.User.Id == userId).Score : 0;
                viewModel.UserFavourite = sighting.Groups.Any(x => x.Group.Id == favouritesId);
                viewModel.IsOwner = sighting.User.Id == authenticatedUser.Id;
            }

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

        public static object MakeUser(User user, User authenticatedUser, bool fullDetails = false, int? sightingCount = 0, IEnumerable<Observation> sampleObservations = null, int? followerCount = 0)
        {
            Check.RequireNotNull(user, "user");

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = user.Id;
            viewModel.Avatar = user.Avatar;
            viewModel.Name = user.Name;
            viewModel.LatestActivity = user.SessionLatestActivity;
            viewModel.LatestHeartbeat = user.SessionLatestHeartbeat;

            if (fullDetails)
            {
                viewModel.Joined = user.Joined.ToString("d MMM yyyy");
                viewModel.Description = user.Description;
                viewModel.ProjectCount = user.Memberships.Where(x => x.Group.GroupType == "project").Count();
                viewModel.OrganisationCount = user.Memberships.Where(x => x.Group.GroupType == "organisation").Count();
                viewModel.SightingCount = sightingCount;

                if (sampleObservations != null)
                {
                    viewModel.SampleObservations = sampleObservations.Select(x =>
                                                                    new
                                                                    {
                                                                        x.Id,
                                                                        Media = x.PrimaryMedia
                                                                    });
                }
                else
                {
                    viewModel.SampleObservations = new object[] { };
                }

                viewModel.FollowingCount = user.FollowingUsers.Count();
                viewModel.FollowerCount = followerCount;

                if (authenticatedUser != null)
                {
                    if (authenticatedUser.Id == user.Id)
                    {
                        viewModel.IsFollowing = false;
                        viewModel.IsFollowed = false;
                    }
                    else
                    {
                        viewModel.IsFollowing = authenticatedUser.Memberships.Any(x => x.Group.GroupType == "userproject" && x.Group.Id == user.UserProject.Id);
                        viewModel.IsFollowed = user.Memberships.Any(x => x.Group.GroupType == "userproject" && x.Group.Id == authenticatedUser.UserProject.Id);
                    }
                }
            }

            return viewModel;
        }

        public static object MakeGroup(Group group, User authenticatedUser, bool fullDetails = false, int sightingCount = 0, int userCount = 0, int postCount = 0, IEnumerable<Observation> sampleObservations = null)
        {
            Check.RequireNotNull(group, "group");

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = group.Id;
            viewModel.Name = group.Name;
            viewModel.GroupType = group.GroupType;

            if (group is IPublicGroup)
            {
                viewModel.Avatar = ((IPublicGroup)group).Avatar;
                viewModel.CreatedBy = group.User.Id;
            }

            if (fullDetails)
            {
                viewModel.Created = group.CreatedDateTime.ToString("d MMM yyyy");
                viewModel.CreatedDateTimeOrder = group.CreatedDateTime.ToString("yyyyMMddHHmmss");

                if (group is IPublicGroup)
                {
                    viewModel.Background = ((IPublicGroup)group).Background;
                    viewModel.Website = ((IPublicGroup)group).Website;
                    viewModel.Description = ((IPublicGroup)group).Description;
                    viewModel.UserCount = userCount;
                    viewModel.PostCount = postCount;
                }
                if (group is Project)
                {
                    viewModel.SightingCount = sightingCount;
                    viewModel.Categories = ((Project)group).Categories;
                    if (sampleObservations != null)
                    {
                        viewModel.SampleObservations = sampleObservations.Select(x =>
                                                                        new
                                                                        {
                                                                            x.Id,
                                                                            Media = x.PrimaryMedia
                                                                        });
                    }
                    else
                    {
                        viewModel.SampleObservations = new object[] { };
                    }
                }
                if (group is Organisation)
                {
                    viewModel.Categories = ((Organisation)group).Categories;
                }

                if (authenticatedUser != null)
                {
                    viewModel.IsMember = authenticatedUser.Memberships.Any(x => x.Group.Id == group.Id);
                }
            }

            return viewModel;
        }

        public static object MakeObservationMedia(ObservationMedia observationMedia)
        {
            return new
            {
                observationMedia.Description,
                observationMedia.IsPrimaryMedia,
                observationMedia.Licence,
                MediaResource = MakeMediaResource(observationMedia.MediaResource)
            };
        }

        public static object MakeMediaResource(MediaResource mediaResource)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = mediaResource.Id;
            viewModel.Key = mediaResource.Key;
            viewModel.MediaResourceType = mediaResource.MediaResourceType;
            viewModel.UploadedOn = mediaResource.UploadedOn;

            viewModel.User = new
            {
                mediaResource.CreatedByUser.Id,
                mediaResource.CreatedByUser.Name
            };

            if (mediaResource.Metadata != null)
            {
                viewModel.Metadata = mediaResource.Metadata.Select(x => new
                {
                    x.Key,
                    x.Value
                });
            }

            if (mediaResource is ImageMediaResource)
            {
                var imageMediaResource = mediaResource as ImageMediaResource;
                viewModel.Image = new ExpandoObject();

                if (imageMediaResource.Image.Original != null)
                {
                    viewModel.Image.Original = new
                    {
                        imageMediaResource.Image.Original.ExifData,
                        imageMediaResource.Image.Original.Filename,
                        imageMediaResource.Image.Original.Height,
                        imageMediaResource.Image.Original.MimeType,
                        imageMediaResource.Image.Original.Size,
                        imageMediaResource.Image.Original.Uri,
                        imageMediaResource.Image.Original.Width
                    };
                }
                if (imageMediaResource.Image.Square50 != null) viewModel.Image.Square50 = MakeDerivedFile(imageMediaResource.Image.Square50);
                if (imageMediaResource.Image.Square100 != null) viewModel.Image.Square100 = MakeDerivedFile(imageMediaResource.Image.Square100);
                if (imageMediaResource.Image.Square200 != null) viewModel.Image.Square200 = MakeDerivedFile(imageMediaResource.Image.Square200);
                if (imageMediaResource.Image.Constrained240 != null) viewModel.Image.Constrained240 = MakeDerivedFile(imageMediaResource.Image.Constrained240);
                if (imageMediaResource.Image.Constrained480 != null) viewModel.Image.Constrained480 = MakeDerivedFile(imageMediaResource.Image.Constrained480);
                if (imageMediaResource.Image.Constrained600 != null) viewModel.Image.Constrained600 = MakeDerivedFile(imageMediaResource.Image.Constrained600);
                if (imageMediaResource.Image.Full640 != null) viewModel.Image.Full640 = MakeDerivedFile(imageMediaResource.Image.Full640);
                if (imageMediaResource.Image.Full800 != null) viewModel.Image.Full800 = MakeDerivedFile(imageMediaResource.Image.Full800);
                if (imageMediaResource.Image.Full1024 != null) viewModel.Image.Full1024 = MakeDerivedFile(imageMediaResource.Image.Full1024);
                if (imageMediaResource.Image.Small != null) viewModel.Image.Small = MakeDerivedFile(imageMediaResource.Image.Small);
                if (imageMediaResource.Image.Large != null) viewModel.Image.Large = MakeDerivedFile(imageMediaResource.Image.Large);

            }
            if (mediaResource is VideoMediaResource)
            {
                var videoMediaResource = mediaResource as VideoMediaResource;
                viewModel.Video = new ExpandoObject();

                if (videoMediaResource.Video.Original != null)
                {
                    viewModel.Video.Original = new
                    {
                        videoMediaResource.Video.Original.Height,
                        videoMediaResource.Video.Original.Provider,
                        videoMediaResource.Video.Original.ProviderData,
                        videoMediaResource.Video.Original.Uri,
                        videoMediaResource.Video.Original.VideoId,
                        videoMediaResource.Video.Original.Width
                    };
                }
                if (videoMediaResource.Video.OriginalImage != null)
                {
                    viewModel.Video.OriginalImage = new
                    {
                        videoMediaResource.Video.OriginalImage.ExifData,
                        videoMediaResource.Video.OriginalImage.Filename,
                        videoMediaResource.Video.OriginalImage.Height,
                        videoMediaResource.Video.OriginalImage.MimeType,
                        videoMediaResource.Video.OriginalImage.Size,
                        videoMediaResource.Video.OriginalImage.Uri,
                        videoMediaResource.Video.OriginalImage.Width
                    };
                }
                if (videoMediaResource.Video.Square50 != null) viewModel.Video.Square50 = MakeDerivedFile(videoMediaResource.Video.Square50);
                if (videoMediaResource.Video.Square100 != null) viewModel.Video.Square100 = MakeDerivedFile(videoMediaResource.Video.Square100);
                if (videoMediaResource.Video.Square200 != null) viewModel.Video.Square200 = MakeDerivedFile(videoMediaResource.Video.Square200);
                if (videoMediaResource.Video.Constrained240 != null) viewModel.Video.Constrained240 = MakeDerivedFile(videoMediaResource.Video.Constrained240);
                if (videoMediaResource.Video.Constrained480 != null) viewModel.Video.Constrained480 = MakeDerivedFile(videoMediaResource.Video.Constrained480);
                if (videoMediaResource.Video.Constrained600 != null) viewModel.Video.Constrained600 = MakeDerivedFile(videoMediaResource.Video.Constrained600);
                if (videoMediaResource.Video.Full640 != null) viewModel.Video.Full640 = MakeDerivedFile(videoMediaResource.Video.Full640);
                if (videoMediaResource.Video.Full800 != null) viewModel.Video.Full800 = MakeDerivedFile(videoMediaResource.Video.Full800);
                if (videoMediaResource.Video.Full1024 != null) viewModel.Video.Full1024 = MakeDerivedFile(videoMediaResource.Video.Full1024);
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

                if (audioMediaResource.Audio.Square50 != null) viewModel.Audio.Square50 = MakeDerivedFile(audioMediaResource.Audio.Square50);
                if (audioMediaResource.Audio.Square100 != null) viewModel.Audio.Square100 = MakeDerivedFile(audioMediaResource.Audio.Square100);
                if (audioMediaResource.Audio.Square200 != null) viewModel.Audio.Square200 = MakeDerivedFile(audioMediaResource.Audio.Square200);
                if (audioMediaResource.Audio.Constrained240 != null) viewModel.Audio.Constrained240 = MakeDerivedFile(audioMediaResource.Audio.Constrained240);
                if (audioMediaResource.Audio.Constrained480 != null) viewModel.Audio.Constrained480 = MakeDerivedFile(audioMediaResource.Audio.Constrained480);
                if (audioMediaResource.Audio.Constrained600 != null) viewModel.Audio.Constrained600 = MakeDerivedFile(audioMediaResource.Audio.Constrained600);
                if (audioMediaResource.Audio.Full640 != null) viewModel.Audio.Full640 = MakeDerivedFile(audioMediaResource.Audio.Full640);
                if (audioMediaResource.Audio.Full800 != null) viewModel.Audio.Full800 = MakeDerivedFile(audioMediaResource.Audio.Full800);
                if (audioMediaResource.Audio.Full1024 != null) viewModel.Audio.Full1024 = MakeDerivedFile(audioMediaResource.Audio.Full1024);
            }

            return viewModel;
        }

        public static object MakeDerivedFile(DerivedMediaResourceFile derivedMediaResourceFile)
        {
            return new
            {
                derivedMediaResourceFile.Height,
                derivedMediaResourceFile.Uri,
                derivedMediaResourceFile.Width
            };
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
                    writer.WriteLine(item.ToString());
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
}