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

            public Observation Observation { get; set; }
            public Record Record { get; set; }
            public Post Post { get; set; }
            public User User { get; set; }
            public SightingNote Note
            {
                get
                {
                    if (Contribution is Sighting)
                    {
                        return ((Sighting) Contribution).Notes.Single(x => x.Id == ContributionSubId);
                    }
                    return null;
                }
            }
            public CommentNew Comment
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
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id)
                                                });

            AddMap<Record>(records => from record in records
                                                 select new
                                                 {
                                                     ContributionId = record.Id,
                                                     ContributionSubId = (string)null,
                                                     ContributionType = "record",
                                                     UserId = record.User.Id,
                                                     CreatedDateTime = record.CreatedOn,
                                                     GroupIds = record.Groups.Select(x => x.Group.Id)
                                                 });

            AddMap<Post>(posts => from post in posts
                                  select new
                                  {
                                      ContributionId = post.Id,
                                      ContributionSubId = (string)null,
                                      ContributionType = "post",
                                      UserId = post.User.Id,
                                      CreatedDateTime = post.CreatedOn,
                                      GroupIds = new[] { post.Group.Id }
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
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id)
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
                                          GroupIds = record.Groups.Select(x => x.Group.Id)
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
                                                    GroupIds = observation.Groups.Select(x => x.Group.Id)
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
                                          GroupIds = record.Groups.Select(x => x.Group.Id)
                                      });

            TransformResults = (database, results) =>
                from result in results
                let observation = database.Load<Observation>(result.ContributionId)
                let record = database.Load<Record>(result.ContributionId)
                let post = database.Load<Post>(result.ContributionId)
                let user = database.Load<User>(result.UserId)
                select new
                {
                    result.ContributionId,
                    result.ContributionType,
                    result.UserId,
                    result.CreatedDateTime,
                    result.GroupIds,
                    Observation = observation,
                    Record = record,
                    Post = post,
                    User = user
                };

            Store(x => x.ContributionId, FieldStorage.Yes);
            Store(x => x.ContributionSubId, FieldStorage.Yes);
            Store(x => x.ContributionType, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.GroupIds, FieldStorage.Yes);
        }
    }
}
