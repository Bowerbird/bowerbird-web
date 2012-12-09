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
            public string ContributionSubId { get; set; }
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
            public IEnumerable<Project> Projects { get; set; }
            public IEnumerable<Team> Teams { get; set; }
            public IEnumerable<Organisation> Organisations { get; set; }
            public SightingNote Note
            {
                get
                {
                    if (Contribution is Sighting)
                    {
                        return ((Sighting) Contribution).Notes.Single(x => x.Id.ToString() == ContributionSubId);
                    }
                    return null;
                }
            }
            public Comment Comment
            {
                get
                {
                    if (Contribution is IDiscussable)
                    {
                        return ((IDiscussable)Contribution).Discussion.Comments.Single(x => x.Id == ContributionSubId);
                    }
                    return null;
                }
            }
            public object Contribution 
            {
                get { return Observation as object ?? Record as object ?? Post as object; }
            }
            public IDiscussable Discussable
            {
                get { return Observation as IDiscussable ?? Record as IDiscussable ?? Post as IDiscussable; }
            }
            public IEnumerable<Group> Groups
            {
                get
                {
                    List<Group> groups = new List<Group>();
                    if (UserProjects != null && UserProjects.Count() > 0) groups.AddRange(UserProjects);
                    if (Projects != null && Projects.Count() > 0) groups.AddRange(Projects);
                    if (Teams != null && Teams.Count() > 0) groups.AddRange(Teams);
                    if (Organisations != null && Organisations.Count() > 0) groups.AddRange(Organisations);
                    return groups;
                }
            }
        }

        public All_Contributions()
        {
            AddMap<Observation>(observations => from observation in observations
                                                select new
                                                {
                                                    ContributionId = observation.Id,
                                                    ContributionSubId = (string)null,
                                                    ContributionType = "observation",
                                                    UserId = observation.User.Id,
                                                    CreatedDateTime = observation.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = observation.Title,
                                                    SightingSightedOn = observation.ObservedOn
                                                });

            AddMap<Record>(records => from record in records
                                                 select new
                                                 {
                                                     ContributionId = record.Id,
                                                     ContributionSubId = (string)null,
                                                     ContributionType = "record",
                                                     UserId = record.User.Id,
                                                     CreatedDateTime = record.CreatedOn,
                                                     GroupIds = record.Groups.Select(x => x.Group.Id),
                                                     SightingTitle = (string)null,
                                                     SightingSightedOn = record.ObservedOn
                                                 });

            AddMap<Post>(posts => from post in posts
                                  select new
                                  {
                                      ContributionId = post.Id,
                                      ContributionSubId = (string)null,
                                      ContributionType = "post",
                                      UserId = post.User.Id,
                                      CreatedDateTime = post.CreatedOn,
                                      GroupIds = new[] { post.Group.Id },
                                      SightingTitle = (string)null,
                                      SightingSightedOn = (object)null
                                  });

            AddMap<Observation>(observations => from observation in observations
                                                from note in observation.Notes
                                                select new
                                                {
                                                    ContributionId = observation.Id,
                                                    ContributionSubId = note.Id,
                                                    ContributionType = "note",
                                                    UserId = note.User.Id,
                                                    CreatedDateTime = note.CreatedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null
                                                });

            AddMap<Record>(records => from record in records
                                      from note in record.Notes
                                      select new
                                      {
                                          ContributionId = record.Id,
                                          ContributionSubId = note.Id,
                                          ContributionType = "note",
                                          UserId = note.User.Id,
                                          CreatedDateTime = note.CreatedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null
                                      });

            AddMap<Observation>(observations => from observation in observations
                                                from comment in observation.Discussion.Comments
                                                select new
                                                {
                                                    ContributionId = observation.Id,
                                                    ContributionSubId = comment.Id,
                                                    ContributionType = "comment",
                                                    UserId = comment.User.Id,
                                                    CreatedDateTime = comment.CommentedOn,
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id),
                                                    SightingTitle = (string)null,
                                                    SightingSightedOn = (object)null
                                                });

            AddMap<Record>(records => from record in records
                                      from comment in record.Discussion.Comments
                                      select new
                                      {
                                          ContributionId = record.Id,
                                          ContributionSubId = comment.Id,
                                          ContributionType = "comment",
                                          UserId = comment.User.Id,
                                          CreatedDateTime = comment.CommentedOn,
                                          GroupIds = record.Groups.Select(x => x.Group.Id),
                                          SightingTitle = (string)null,
                                          SightingSightedOn = (object)null
                                      });

            TransformResults = (database, results) =>
                from result in results
                select new
                {
                    result.ContributionId,
                    result.ContributionType,
                    result.ContributionSubId,
                    result.UserId,
                    result.CreatedDateTime,
                    GroupIds = result.GroupIds ?? new string[] {},
                    Observation = database.Load<Observation>(result.ContributionId),
                    Record = database.Load<Record>(result.ContributionId),
                    Post = database.Load<Post>(result.ContributionId),
                    User = database.Load<User>(result.UserId),
                    UserProjects = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "userproject"),
                    Projects = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "project"),
                    Teams = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "team"),
                    Organisations = database.Load<UserProject>(result.GroupIds).Where(x => x.GroupType == "organisation")
                };

            Store(x => x.ContributionId, FieldStorage.Yes);
            Store(x => x.ContributionSubId, FieldStorage.Yes);
            Store(x => x.ContributionType, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.GroupIds, FieldStorage.Yes);
            Store(x => x.SightingTitle, FieldStorage.Yes);
            Store(x => x.SightingSightedOn, FieldStorage.Yes);
        }
    }
}
