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
            public string ContributionId { get; set; }
            public string UserId { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public string GroupId { get; set; }
            public string GroupUserId { get; set; }
            public DateTime GroupCreatedDateTime { get; set; }
            public string ContributionType { get; set; }
            public Observation Observation { get; set; }
            public ObservationNote ObservationNote { get; set; }
            public Post Post { get; set; }
            public User User { get; set; }
            public User GroupUser { get; set; }
        }

        public All_Contributions()
        {
            AddMap<Observation>(observations =>
                from c in observations
                from gc in c.Groups
                where gc.Group.GroupType != "userproject"
                select new
                {
                    ContributionId = c.Id,
                    ContributionType = "Observation",
                    UserId = c.User.Id,
                    CreatedDateTime = c.CreatedOn,
                    GroupId = gc.Group.Id,
                    GroupUserId = gc.User.Id,
                    GroupCreatedDateTime = gc.CreatedDateTime
                });

            AddMap<Post>(posts => 
                from c in posts
                select new
                {
                    ContributionId = c.Id,
                    ContributionType = "Post",
                    UserId = c.User.Id,
                    CreatedDateTime = c.CreatedOn,
                    c.GroupId,
                    GroupUserId = c.User.Id,
                    GroupCreatedDateTime = c.CreatedOn
                });

            AddMap<Observation>(observations =>
                from o in observations
                from og in o.Groups
                where og.Group.GroupType != "userproject"
                from n in o.Notes
                select new
                {
                    ContributionId = n.Id,
                    ContributionType = "ObservationNote",
                    UserId = n.UserId,
                    CreatedDateTime = n.CreatedOn,
                    GroupId = og.Group.Id,
                    GroupUserId = og.User.Id,
                    GroupCreatedDateTime = og.CreatedDateTime
                });

            TransformResults = (database, results) =>
                from result in results
                let observation = database.Load<Observation>(result.ContributionId)
                let observationNote = database.Load<ObservationNote>(result.ContributionId)
                let post = database.Load<Post>(result.ContributionId)
                let user = database.Load<User>(result.UserId)
                let groupUser = database.Load<User>(result.GroupUserId)
                select new
                {
                    result.ContributionId,
                    result.ContributionType,
                    result.UserId,
                    result.CreatedDateTime,
                    result.GroupId,
                    result.GroupUserId,
                    result.GroupCreatedDateTime,
                    Observation = observation,
                    ObservationNote = observationNote,
                    Post = post,
                    User = user,
                    GroupUser = groupUser
                };

            Store(x => x.ContributionId, FieldStorage.Yes);
            Store(x => x.ContributionType, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.GroupUserId, FieldStorage.Yes);
            Store(x => x.GroupCreatedDateTime, FieldStorage.Yes);
        }
    }
}