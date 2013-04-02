    /* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com

 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au

 Funded by:
 * Atlas of Living Australia

*/

using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class All_Contributions : AbstractMultiMapIndexCreationTask<All_Contributions.Result>
    {
        public class Result
        {
            public string ParentContributionId { get; set; }
            public string SubContributionId { get; set; }
            public string ParentContributionType { get; set; }
            public string SubContributionType { get; set; }
            public string UserId { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public string[] GroupIds { get; set; }

            // Sightings
            public string SightingTitle { get; set; }
            public object SightingSightedOn { get; set; }
            public string SightingCategory { get; set; }
            public object[] SightingTags { get; set; }
            public object[] SightingDescriptions { get; set; }
            public int? SightingIdentificationCount { get; set; }
            public object[] SightingAllFields { get; set; }
            public object[] SightingTaxonomicRanks { get; set; }
            public string SightingSortTitle { get; set; }
            public int? SightingVoteCount { get; set; }

            // Posts
            public string PostTitle { get; set; }
            public string PostMessage { get; set; }
            public object[] PostAllFields { get; set; }
            public string PostSortTitle { get; set; }

            public Observation Observation { get; set; }
            public Record Record { get; set; }
            public Post Post { get; set; }
            public User User { get; set; }
            public IEnumerable<UserProject> UserProjects { get; set; }
            public IEnumerable<Favourites> Favourites { get; set; }
            public IEnumerable<Project> Projects { get; set; }
            public IEnumerable<Organisation> Organisations { get; set; }

            public IContribution Contribution 
            {
                get
                {
                    if (SubContributionType == "identification" || SubContributionType == "note" || SubContributionType == "vote")
                    {
                        return ParentContribution.GetSubContribution(SubContributionType, SubContributionId);
                    }

                    return ParentContribution;
                }
            }

            public IContribution ParentContribution
            {
                get
                {
                    return Observation as IContribution ??
                           Record as IContribution ??
                           Post as IContribution;
                }
            }

            public IEnumerable<Group> Groups
            {
                get
                {
                    List<Group> groups = new List<Group>();
                    if (UserProjects != null) groups.AddRange(UserProjects);
                    if (Favourites != null) groups.AddRange(Favourites);
                    if (Projects != null) groups.AddRange(Projects);
                    if (Organisations != null) groups.AddRange(Organisations);
                    return groups;
                }
            }
        }

        public All_Contributions()
        {
            // Observations
            AddMap<Observation>(observations => from observation in observations
                                                select new
                                                {
                                                    ParentContributionId = observation.Id,
                                                    SubContributionId = (string)null,
                                                    ParentContributionType = "observation",
                                                    SubContributionType = (string)null,
                                                    UserId = observation.User.Id,
                                                    CreatedDateTime = observation.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = observation.Title,  
                                                    SightingSightedOn = observation.ObservedOn,
                                                    SightingCategory = observation.Category,
                                                    SightingTags = observation.Notes.SelectMany(x => x.Tags),
                                                    SightingDescriptions = observation.Notes.SelectMany(x => x.Descriptions.Select(y => y.Text)),
                                                    SightingIdentificationCount = observation.Identifications.Count(),
                                                    SightingAllFields = new object[]
                                                        {
                                                            observation.Title,
                                                            observation.Notes.SelectMany(x => x.Descriptions.Select(y => y.Text)),
                                                            observation.Notes.SelectMany(x => x.Tags),
                                                            observation.Identifications.SelectMany(x => x.TaxonomicRanks.Select(y => y.Name)),
                                                            observation.Identifications.SelectMany(x => x.CommonNames),
                                                            observation.Identifications.SelectMany(x => x.CommonGroupNames)
                                                        },
                                                    SightingTaxonomicRanks = new object[]
                                                        {
                                                            observation.Identifications.SelectMany(x => x.TaxonomicRanks.Select(y => y.Name))  
                                                        },
                                                    SightingVoteCount = observation.Votes.Sum(x => x.Score),
                                                    PostTitle = (string)null,
                                                    PostMessage = (string)null,
                                                    PostAllFields = new object[] {},
                                                    SightingSortTitle = (string)null,
                                                    PostSortTitle = (string)null
                                                });

            // Records
            AddMap<Record>(records => from record in records
                                      select new
                                          {
                                              ParentContributionId = record.Id,
                                              SubContributionId = (string) null,
                                              ParentContributionType = "record",
                                              SubContributionType = (string) null,
                                              UserId = record.User.Id,
                                              CreatedDateTime = record.CreatedOn,
                                              GroupIds = record.Groups.Select(x => x.Group.Id),
                                              SightingTitle = (string) null,
                                              SightingSightedOn = record.ObservedOn,
                                              SightingCategory = record.Category,
                                              SightingTags = new object[] {},
                                              SightingDescriptions = new object[] {},
                                              SightingIdentificationCount = 1,
                                              SightingAllFields = new object[]
                                                  {
                                                      record.Identifications.SelectMany(x => x.TaxonomicRanks.Select(y => y.Name)),
                                                      record.Identifications.SelectMany(x => x.CommonNames),
                                                      record.Identifications.SelectMany(x => x.CommonGroupNames)
                                                  },
                                              SightingTaxonomicRanks = new object[]
                                                  {
                                                      record.Identifications.SelectMany(x => x.TaxonomicRanks.Select(y => y.Name))
                                                  },
                                              SightingVoteCount = record.Votes.Sum(x => x.Score),
                                              PostTitle = (string)null,
                                              PostMessage = (string)null,
                                              PostAllFields = new object[] { },
                                              SightingSortTitle = (string)null,
                                              PostSortTitle = (string)null
                                          });

            // Posts
            AddMap<Post>(posts => from post in posts
                                  select new
                                  {
                                      ParentContributionId = post.Id,
                                      SubContributionId = (string)null,
                                      ParentContributionType = "post",
                                      SubContributionType = (string)null,
                                      UserId = post.User.Id,
                                      CreatedDateTime = post.CreatedOn,
                                      GroupIds = new[] { post.Group.Id },
                                      SightingTitle = (string)null,
                                      SightingSightedOn = (object)null,
                                      SightingCategory = (string)null,
                                      SightingTags = new object[] { },
                                      SightingDescriptions = new object[] { },
                                      SightingIdentificationCount = (object)null,
                                      SightingAllFields = new object[] { },
                                      SightingTaxonomicRanks = new object[] {},
                                      SightingVoteCount = (object)null,
                                      PostTitle = post.Subject,
                                      PostMessage = post.Message,
                                      PostAllFields = new object[]
                                          {
                                              post.Subject,
                                              post.Message
                                          },
                                      SightingSortTitle = (string)null,
                                      PostSortTitle = (string)null
                                  });

            // Observation Notes
            AddMap<Observation>(observations => from observation in observations
                                                from note in observation.Notes
                                                select new
                                                {
                                                    ParentContributionId = observation.Id,
                                                    SubContributionId = note.Id,
                                                    ParentContributionType = "observation",
                                                    SubContributionType = "note",
                                                    UserId = note.User.Id,
                                                    CreatedDateTime = note.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null,
                                                    SightingCategory = (string)null,
                                                    SightingTags = new object[] { },
                                                    SightingDescriptions = new object[] { },
                                                    SightingIdentificationCount = (object)null,
                                                    SightingAllFields = new object[] { },
                                                    SightingTaxonomicRanks = new object[] { },
                                                    SightingVoteCount = (object)null,
                                                    PostTitle = (string)null,
                                                    PostMessage = (string)null,
                                                    PostAllFields = new object[] { },
                                                    SightingSortTitle = (string)null,
                                                    PostSortTitle = (string)null
                                                });

            // Record Notes
            AddMap<Record>(records => from record in records
                                      from note in record.Notes
                                      select new
                                      {
                                          ParentContributionId = record.Id,
                                          SubContributionId = note.Id,
                                          ParentContributionType = "record",
                                          SubContributionType = "note",
                                          UserId = note.User.Id,
                                          CreatedDateTime = note.CreatedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null,
                                          SightingCategory = (string)null,
                                          SightingTags = new object[] { },
                                          SightingDescriptions = new object[] { },
                                          SightingIdentificationCount = (object)null,
                                          SightingAllFields = new object[] { },
                                          SightingTaxonomicRanks = new object[] { },
                                          SightingVoteCount = (object)null,
                                          PostTitle = (string)null,
                                          PostMessage = (string)null,
                                          PostAllFields = new object[] { },
                                          SightingSortTitle = (string)null,
                                          PostSortTitle = (string)null
                                      });

            // Observation Comments
            AddMap<Observation>(observations => from observation in observations
                                                from comment in observation.Discussion.Comments
                                                select new
                                                {
                                                    ParentContributionId = observation.Id,
                                                    SubContributionId = comment.Id,
                                                    ParentContributionType = "observation",
                                                    SubContributionType = "comment",
                                                    UserId = comment.User.Id,
                                                    CreatedDateTime = comment.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null,
                                                    SightingCategory = (string)null,
                                                    SightingTags = new object[] { },
                                                    SightingDescriptions = new object[] { },
                                                    SightingIdentificationCount = (object)null,
                                                    SightingAllFields = new object[] { },
                                                    SightingTaxonomicRanks = new object[] { },
                                                    SightingVoteCount = (object)null,
                                                    PostTitle = (string)null,
                                                    PostMessage = (string)null,
                                                    PostAllFields = new object[] { },
                                                    SightingSortTitle = (string)null,
                                                    PostSortTitle = (string)null
                                                });

            // Record Comments
            AddMap<Record>(records => from record in records
                                      from comment in record.Discussion.Comments
                                      select new
                                      {
                                          ParentContributionId = record.Id,
                                          SubContributionId = comment.Id,
                                          ParentContributionType = "record",
                                          SubContributionType = "comment",
                                          UserId = comment.User.Id,
                                          CreatedDateTime = comment.CreatedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null,
                                          SightingCategory = (string)null,
                                          SightingTags = new object[] { },
                                          SightingDescriptions = new object[] { },
                                          SightingIdentificationCount = (object)null,
                                          SightingAllFields = new object[] { },
                                          SightingTaxonomicRanks = new object[] { },
                                          SightingVoteCount = (object)null,
                                          PostTitle = (string)null,
                                          PostMessage = (string)null,
                                          PostAllFields = new object[] { },
                                          SightingSortTitle = (string)null,
                                          PostSortTitle = (string)null
                                      });

            // Observation Identifications
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                select new
                                                {
                                                    ParentContributionId = observation.Id,
                                                    SubContributionId = identification.Id,
                                                    ParentContributionType = "observation",
                                                    SubContributionType = "identification",
                                                    UserId = identification.User.Id,
                                                    CreatedDateTime = identification.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null,
                                                    SightingCategory = (string)null,
                                                    SightingTags = new object[] { },
                                                    SightingDescriptions = new object[] { },
                                                    SightingIdentificationCount = (object)null,
                                                    SightingAllFields = new object[] { },
                                                    SightingTaxonomicRanks = new object[] { },
                                                    SightingVoteCount = (object)null,
                                                    PostTitle = (string)null,
                                                    PostMessage = (string)null,
                                                    PostAllFields = new object[] { },
                                                    SightingSortTitle = (string)null,
                                                    PostSortTitle = (string)null
                                                });

            // Record Identifications
            AddMap<Record>(records => from record in records
                                      from identification in record.Identifications
                                      select new
                                      {
                                          ParentContributionId = record.Id,
                                          SubContributionId = identification.Id,
                                          ParentContributionType = "record",
                                          SubContributionType = "identification",
                                          UserId = identification.User.Id,
                                          CreatedDateTime = identification.CreatedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null,
                                          SightingCategory = (string)null,
                                          SightingTags = new object[] { },
                                          SightingDescriptions = new object[] { },
                                          SightingIdentificationCount = (object)null,
                                          SightingAllFields = new object[] { },
                                          SightingTaxonomicRanks = new object[] { },
                                          SightingVoteCount = (object)null,
                                          PostTitle = (string)null,
                                          PostMessage = (string)null,
                                          PostAllFields = new object[] { },
                                          SightingSortTitle = (string)null,
                                          PostSortTitle = (string)null
                                      });

            // Observation Votes
            AddMap<Observation>(observations => from observation in observations
                                                from vote in observation.Votes
                                                select new
                                                    {
                                                        ParentContributionId = observation.Id,
                                                        SubContributionId = vote.Id,
                                                        ParentContributionType = "observation",
                                                        SubContributionType = "vote",
                                                        UserId = vote.User.Id,
                                                        CreatedDateTime = vote.CreatedOn,
                                                        GroupIds = new string[] {},
                                                        SightingTitle = (string) null,
                                                        SightingSightedOn = (object) null,
                                                        SightingCategory = (string)null,
                                                        SightingTags = new object[] { },
                                                        SightingDescriptions = new object[] { },
                                                        SightingIdentificationCount = (object)null,
                                                        SightingAllFields = new object[] { },
                                                        SightingTaxonomicRanks = new object[] { },
                                                        SightingVoteCount = (object)null,
                                                        PostTitle = (string)null,
                                                        PostMessage = (string)null,
                                                        PostAllFields = new object[] { },
                                                        SightingSortTitle = (string)null,
                                                        PostSortTitle = (string)null
                                                    });

            // Record Votes
            AddMap<Record>(records => from record in records
                                      from vote in record.Votes
                                      select new
                                          {
                                              ParentContributionId = record.Id,
                                              SubContributionId = vote.Id,
                                              ParentContributionType = "record",
                                              SubContributionType = "vote",
                                              UserId = vote.User.Id,
                                              CreatedDateTime = vote.CreatedOn,
                                              GroupIds = new string[] {},
                                              SightingTitle = (string) null,
                                              SightingSightedOn = (object) null,
                                              SightingCategory = (string)null,
                                              SightingTags = new object[] { },
                                              SightingDescriptions = new object[] { },
                                              SightingIdentificationCount = (object)null,
                                              SightingAllFields = new object[] { },
                                              SightingTaxonomicRanks = new object[] { },
                                              SightingVoteCount = (object)null,
                                              PostTitle = (string)null,
                                              PostMessage = (string)null,
                                              PostAllFields = new object[] { },
                                              SightingSortTitle = (string)null,
                                              PostSortTitle = (string)null
                                          });

            Reduce = (results => from result in results
                                 group result by new { result.ParentContributionId, result.SubContributionId, result.ParentContributionType, result.SubContributionType }
                                 into g
                                 select new
                                     {
                                        g.Key.ParentContributionId,
                                        g.Key.SubContributionId,
                                        g.Key.ParentContributionType,
                                        g.Key.SubContributionType,
                                        UserId = g.Select(x => x.UserId).Where(x => x != null).FirstOrDefault(),
                                        CreatedDateTime = g.Select(x => x.CreatedDateTime).FirstOrDefault(),
                                        GroupIds = g.SelectMany(x => x.GroupIds),
                                        SightingTitle = g.Select(x => x.SightingTitle).Where(x => x != null).FirstOrDefault(),
                                        SightingSightedOn = g.Select(x => x.SightingSightedOn).FirstOrDefault(),
                                        SightingCategory = g.Select(x => x.SightingCategory).Where(x => x != null).FirstOrDefault(),
                                        SightingTags = g.SelectMany(x => x.SightingTags),
                                        SightingDescriptions = g.SelectMany(x => x.SightingDescriptions),
                                        SightingIdentificationCount = g.Select(x => x.SightingIdentificationCount).Where(x => x != null).FirstOrDefault(),
                                        SightingAllFields = g.SelectMany(x => x.SightingAllFields),
                                        SightingTaxonomicRanks = g.SelectMany(x => x.SightingTaxonomicRanks),
                                        SightingVoteCount = g.Select(x => x.SightingVoteCount).Where(x => x != null).FirstOrDefault(),
                                        PostTitle = g.Select(x => x.PostTitle).Where(x => x != null).FirstOrDefault(),
                                        PostMessage = g.Select(x => x.PostMessage).Where(x => x != null).FirstOrDefault(),
                                        PostAllFields = g.SelectMany(x => x.PostAllFields),
                                        SightingSortTitle = g.Select(x => x.SightingTitle).Where(x => x != null).FirstOrDefault(),
                                        PostSortTitle = g.Select(x => x.PostTitle).Where(x => x != null).FirstOrDefault()
                                     });

            TransformResults = (database, results) =>
                from result in results
                select new
                {
                    result.ParentContributionId,
                    result.SubContributionId,
                    result.ParentContributionType,
                    result.SubContributionType,
                    result.UserId,
                    result.CreatedDateTime,
                    GroupIds = result.GroupIds ?? new string[] {},
                    Observation = result.ParentContributionType == "observation" ? database.Load<Observation>(result.ParentContributionId) : null,
                    Record = result.ParentContributionType == "record" ? database.Load<Record>(result.ParentContributionId) : null,
                    Post = result.ParentContributionType == "post" ? database.Load<Post>(result.ParentContributionId) : null,
                    User = database.Load<User>(result.UserId),
                    UserProjects = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "userproject"),
                    Favourites = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "favourites"),
                    Projects = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "project"),
                    Organisations = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "organisation")
                };

            Store(x => x.ParentContributionId, FieldStorage.Yes);
            Store(x => x.SubContributionId, FieldStorage.Yes);
            Store(x => x.ParentContributionType, FieldStorage.Yes);
            Store(x => x.SubContributionType, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.GroupIds, FieldStorage.Yes);
            Store(x => x.SightingTitle, FieldStorage.No);
            Store(x => x.SightingSightedOn, FieldStorage.No);
            Store(x => x.SightingCategory, FieldStorage.No);
            Store(x => x.SightingTags, FieldStorage.No);
            Store(x => x.SightingDescriptions, FieldStorage.No);
            Store(x => x.SightingIdentificationCount, FieldStorage.No);
            Store(x => x.SightingAllFields, FieldStorage.No);
            Store(x => x.SightingTaxonomicRanks, FieldStorage.No);
            Store(x => x.SightingVoteCount, FieldStorage.No);
            Store(x => x.PostTitle, FieldStorage.No);
            Store(x => x.PostMessage, FieldStorage.No);
            Store(x => x.PostAllFields, FieldStorage.No);
            Store(x => x.SightingSortTitle, FieldStorage.Yes);
            Store(x => x.PostSortTitle, FieldStorage.Yes);

            Index(x => x.ParentContributionId, FieldIndexing.Analyzed);
            Index(x => x.SubContributionId, FieldIndexing.Analyzed);
            Index(x => x.ParentContributionType, FieldIndexing.Analyzed);
            Index(x => x.SubContributionType, FieldIndexing.Analyzed);
            //Index(x => x.UserId, FieldIndexing.Analyzed);
            //Index(x => x.GroupIds, FieldIndexing.Analyzed);

            Index(x => x.SightingTitle, FieldIndexing.Analyzed);
            Index(x => x.SightingTags, FieldIndexing.Analyzed);
            Index(x => x.SightingDescriptions, FieldIndexing.Analyzed);
            Index(x => x.SightingAllFields, FieldIndexing.Analyzed);
            Index(x => x.SightingTaxonomicRanks, FieldIndexing.Analyzed);
            Index(x => x.PostTitle, FieldIndexing.Analyzed);
            Index(x => x.PostMessage, FieldIndexing.Analyzed);
            Index(x => x.PostAllFields, FieldIndexing.Analyzed);
        }
    }
}
