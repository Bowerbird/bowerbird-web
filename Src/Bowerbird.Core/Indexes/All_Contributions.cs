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
            public string UserId { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public string[] GroupIds { get; set; }

            public Observation Observation { get; set; }
            public ObservationNote ObservationNote { get; set; }
            public Post Post { get; set; }
            public User User { get; set; }
        }

        public All_Contributions()
        {
            AddMap<Observation>(observations => from observation in observations
                select new
                {
                    ContributionId = observation.Id,
                    ContributionType = "observation",
                    UserId = observation.User.Id,
                    CreatedDateTime = observation.CreatedOn,
                    GroupIds = observation.Groups.Select(x => x.Group.Id)
                });

            AddMap<Post>(posts => from post in posts
                select new
                {
                    ContributionId = post.Id,
                    ContributionType = "post",
                    UserId = post.User.Id,
                    CreatedDateTime = post.CreatedOn,
                    GroupIds = new [] { post.Group.Id }
                });

            AddMap<Observation>(observations => 
                from observation in observations
                from note in observation.Notes
                select new
                {
                    ContributionId = note.Id,
                    ContributionType = "observationnote",
                    UserId = note.UserId,
                    CreatedDateTime = note.CreatedOn,
                    GroupIds = observation.Groups.Select(x => x.Group.Id)
                });

            TransformResults = (database, results) =>
                from result in results
                let observation = database.Load<Observation>(result.ContributionId)
                let observationNote = database.Load<ObservationNote>(result.ContributionId)
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
                    ObservationNote = observationNote,
                    Post = post,
                    User = user
                };

            Store(x => x.ContributionId, FieldStorage.Yes);
            Store(x => x.ContributionType, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.GroupIds, FieldStorage.Yes);
        }
    }
}