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
    public class ContributionResults
    {
        public string ContributionId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

    public class All_Contributions : AbstractMultiMapIndexCreationTask<ContributionResults>
    {
        public All_Contributions()
        {

            AddMap<Post>(posts => from post in posts
                                  select new
                                             {
                                                 ContributionId = post.Id,
                                                 CreatedDateTime = post.CreatedOn,
                                                 UserId = post.User.Id,
                                             });

            AddMap<Observation>(observations => from observation in observations
                                                select new
                                                           {
                                                               ContributionId = observation.Id,
                                                               CreatedDateTime = observation.CreatedOn,
                                                               UserId = observation.User.Id
                                                           });

            AddMap<ObservationNote>(observationNotes => from observationNote in observationNotes
                                                        select new
                                                                   {
                                                                       ContributionId = observationNote.Id,
                                                                       CreatedDateTime =
                                                            observationNote.CreatedOn,
                                                                       UserId = observationNote.User.Id
                                                                   });
        }
    }

    public class GroupContributionResults
    {
        public string ContributionId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string GroupId { get; set; }
        public string GroupUserId { get; set; }
        public DateTime GroupCreatedDateTime { get; set; }
        public Observation Observation { get; set; }
        public ObservationNote ObservationNote { get; set; }
        public Post Post { get; set; }
        public Contribution Contribution
        {
            get { return (Contribution)Observation ?? (Contribution)ObservationNote ?? (Contribution)Post; }
        }
    }

    public class All_GroupContributionItems : AbstractMultiMapIndexCreationTask<GroupContributionResults>
    {
        public All_GroupContributionItems()
        {
            AddMap<Observation>(observations => 
                from c in observations
                from gc in c.GroupContributions
                select new
                {
                    ContributionId = c.Id,
                    UserId = c.User.Id,
                    CreatedDateTime = c.CreatedOn,
                    gc.GroupId,
                    GroupUserId = gc.User.Id,
                    GroupCreatedDateTime = gc.CreatedDateTime
                });

            AddMap<Post>(posts => 
                from c in posts
                from gc in c.GroupContributions
                select new
                {
                    ContributionId = c.Id,
                    UserId = c.User.Id,
                    CreatedDateTime = c.CreatedOn,
                    gc.GroupId,
                    GroupUserId = gc.User.Id,
                    GroupCreatedDateTime = gc.CreatedDateTime
                });

            AddMap<ObservationNote>(observationNotes => 
                from c in observationNotes
                from gc in c.GroupContributions
                select new
                {
                    ContributionId = c.Id,
                    UserId = c.User.Id,
                    CreatedDateTime = c.CreatedOn,
                    gc.GroupId,
                    GroupUserId = gc.User.Id,
                    GroupCreatedDateTime = gc.CreatedDateTime
                });

            TransformResults = (database, results) => 
                from result in results
                let observation = database.Load<Observation>(result.ContributionId)
                let observationNote = database.Load<ObservationNote>(result.ContributionId)
                let post = database.Load<Post>(result.ContributionId)
                select new
                {
                    result.ContributionId,
                    result.UserId,
                    result.CreatedDateTime,
                    result.GroupId,
                    result.GroupUserId,
                    result.GroupCreatedDateTime,
                    Observation = observation,
                    ObservationNote = observationNote,
                    Post = post
                };

            Store(x => x.ContributionId, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.GroupUserId, FieldStorage.Yes);
            Store(x => x.GroupCreatedDateTime, FieldStorage.Yes);
        }
    }
}