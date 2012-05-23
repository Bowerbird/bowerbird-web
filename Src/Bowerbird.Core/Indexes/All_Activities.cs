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
    public class All_Activities : AbstractMultiMapIndexCreationTask<All_Activities.Result>
    {
        public class Result
        {
            public string ActivityId { get; set; }
            public string ActivityType { get; set; }
            public string UserId { get; set; }
            public DateTime CreatedDateTime { get; set; }

            public Activity Activity { get; set; }
        }

        public All_Activities()
        {
            AddMap<Activity>(activities => from activity in activities
                select new
                {
                    ActivityId = activity.Id,
                    ActivityType = activity.Type,
                    activity.UserId,
                    CreatedDateTime = activity.CreatedDateTime
                });

            TransformResults = (database, results) =>
                from result in results
                let activity = database.Load<Activity>(result.ActivityId)
                select new
                {
                    result.ActivityId,
                    result.ActivityType,
                    result.UserId,
                    result.CreatedDateTime,
                    Activity = activity
                };

            Store(x => x.ActivityId, FieldStorage.Yes);
            Store(x => x.ActivityType, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
        }
    }
}