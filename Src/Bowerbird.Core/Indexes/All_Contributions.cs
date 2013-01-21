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
            public string ContributionType { get; set; }
            public string ContributionId { get; set; }
            public string SubContributionId { get; set; }
            public string UserId { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public string[] GroupIds { get; set; }

            // Sighting specific
            public string SightingTitle { get; set; }
            public object SightingSightedOn { get; set; }

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
                    var contribution = Observation as IContribution ?? Record as IContribution ?? Post as IContribution;

                    if (ContributionType == "observation" || ContributionType == "record" || ContributionType == "post")
                    {
                        return contribution;
                    }

                    if (ContributionType == "identification" || ContributionType == "note" || ContributionType == "vote")
                    {
                        return contribution.GetSubContribution(ContributionType, SubContributionId);
                    }

                    return null;
                }
            }

            public IContribution ParentContribution
            {
                get
                {
                    if (ContributionType == "identification" || ContributionType == "note" || ContributionType == "vote")
                    {
                        return Observation as IContribution ?? Record as IContribution ?? Post as IContribution;
                    }

                    return null;
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
                                                    ContributionId = observation.Id,
                                                    SubContributionId = (string)null,
                                                    ContributionType = "observation",
                                                    UserId = observation.User.Id,
                                                    CreatedDateTime = observation.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = observation.Title,
                                                    SightingSightedOn = observation.ObservedOn
                                                });

            // Records
            AddMap<Record>(records => from record in records
                                                 select new
                                                 {
                                                     ContributionId = record.Id,
                                                     SubContributionId = (string)null,
                                                     ContributionType = "record",
                                                     UserId = record.User.Id,
                                                     CreatedDateTime = record.CreatedOn,
                                                     GroupIds = record.Groups.Select(x => x.Group.Id),
                                                     SightingTitle = (string)null,
                                                     SightingSightedOn = record.ObservedOn
                                                 });

            // Posts
            AddMap<Post>(posts => from post in posts
                                  select new
                                  {
                                      ContributionId = post.Id,
                                      SubContributionId = (string)null,
                                      ContributionType = "post",
                                      UserId = post.User.Id,
                                      CreatedDateTime = post.CreatedOn,
                                      GroupIds = new[] { post.Group.Id },
                                      SightingTitle = (string)null,
                                      SightingSightedOn = (object)null
                                  });

            // Observation Notes
            AddMap<Observation>(observations => from observation in observations
                                                from note in observation.Notes
                                                select new
                                                {
                                                    ContributionId = observation.Id,
                                                    SubContributionId = note.Id,
                                                    ContributionType = "note",
                                                    UserId = note.User.Id,
                                                    CreatedDateTime = note.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null
                                                });

            // Record Notes
            AddMap<Record>(records => from record in records
                                      from note in record.Notes
                                      select new
                                      {
                                          ContributionId = record.Id,
                                          SubContributionId = note.Id,
                                          ContributionType = "note",
                                          UserId = note.User.Id,
                                          CreatedDateTime = note.CreatedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null
                                      });

            // Observation Comments
            AddMap<Observation>(observations => from observation in observations
                                                from comment in observation.Discussion.Comments
                                                select new
                                                {
                                                    ContributionId = observation.Id,
                                                    SubContributionId = comment.Id,
                                                    ContributionType = "comment",
                                                    UserId = comment.User.Id,
                                                    CreatedDateTime = comment.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null
                                                });

            // Record Comments
            AddMap<Record>(records => from record in records
                                      from comment in record.Discussion.Comments
                                      select new
                                      {
                                          ContributionId = record.Id,
                                          SubContributionId = comment.Id,
                                          ContributionType = "comment",
                                          UserId = comment.User.Id,
                                          CreatedDateTime = comment.CreatedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null
                                      });

            // Observation Identifications
            AddMap<Observation>(observations => from observation in observations
                                                from identification in observation.Identifications
                                                select new
                                                {
                                                    ContributionId = observation.Id,
                                                    SubContributionId = identification.Id,
                                                    ContributionType = "identification",
                                                    UserId = identification.User.Id,
                                                    CreatedDateTime = identification.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null
                                                });

            // Record Identifications
            AddMap<Record>(records => from record in records
                                      from identification in record.Identifications
                                      select new
                                      {
                                          ContributionId = record.Id,
                                          SubContributionId = identification.Id,
                                          ContributionType = "identification",
                                          UserId = identification.User.Id,
                                          CreatedDateTime = identification.CreatedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null
                                      });

            // Observation Votes
            AddMap<Observation>(observations => from observation in observations
                                                from vote in observation.Votes
                                                select new
                                                    {
                                                        ContributionId = observation.Id,
                                                        SubContributionId = vote.Id,
                                                        ContributionType = "vote",
                                                        UserId = vote.User.Id,
                                                        CreatedDateTime = vote.CreatedOn,
                                                        GroupIds = new string[] {},
                                                        SightingTitle = (string) null,
                                                        SightingSightedOn = (object) null
                                                    });

            // Record Votes
            AddMap<Record>(records => from record in records
                                      from vote in record.Votes
                                      select new
                                          {
                                              ContributionId = record.Id,
                                              SubContributionId = vote.Id,
                                              ContributionType = "vote",
                                              UserId = vote.User.Id,
                                              CreatedDateTime = vote.CreatedOn,
                                              GroupIds = new string[] {},
                                              SightingTitle = (string) null,
                                              SightingSightedOn = (object) null
                                          });

            Reduce = (results => from result in results
                                 group result by new { result.ContributionId, result.SubContributionId, result.ContributionType }
                                 into g
                                 select new
                                     {
                                        g.Key.ContributionId,
                                        g.Key.SubContributionId,
                                        g.Key.ContributionType,
                                        UserId = g.Select(x => x.UserId).Where(x => x != null).FirstOrDefault(),
                                        CreatedDateTime = g.Select(x => x.CreatedDateTime).Where(x => x != null).FirstOrDefault(),
                                        GroupIds = g.SelectMany(x => x.GroupIds),
                                        SightingTitle = g.Select(x => x.SightingTitle).Where(x => x != null).FirstOrDefault(),
                                        SightingSightedOn = g.Select(x => x.SightingSightedOn).Where(x => x != null).FirstOrDefault()
                                     });

            TransformResults = (database, results) =>
                from result in results
                select new
                {
                    result.ContributionType,
                    result.ContributionId,
                    result.SubContributionId,
                    result.UserId,
                    result.CreatedDateTime,
                    GroupIds = result.GroupIds ?? new string[] {},
                    Observation = database.Load<Observation>(result.ContributionId),
                    Record = database.Load<Record>(result.ContributionId),
                    Post = database.Load<Post>(result.ContributionId),
                    User = database.Load<User>(result.UserId),
                    UserProjects = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "userproject"),
                    Favourites = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "favourites"),
                    Projects = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "project"),
                    Organisations = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "organisation")
                };

            Store(x => x.ContributionId, FieldStorage.Yes);
            Store(x => x.SubContributionId, FieldStorage.Yes);
            Store(x => x.ContributionType, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.GroupIds, FieldStorage.Yes);
            Store(x => x.SightingTitle, FieldStorage.Yes);
            Store(x => x.SightingSightedOn, FieldStorage.Yes);
        }
    }
}
