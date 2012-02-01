/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client.Indexes;

namespace Bowerbird.Web.Indexes
{
    public class StreamItem_WithParentIdAndUserIdAndCreatedDateTimeAndType : AbstractIndexCreationTask<StreamItem>
    {
        public StreamItem_WithParentIdAndUserIdAndCreatedDateTimeAndType()
        {
            Map = streamItems => from streamItem in streamItems

                                 select new
                                            {
                                                streamItem.ParentId, 
                                                UserId = streamItem.User.Id, 
                                                streamItem.CreatedDateTime, 
                                                streamItem.Type, 
                                                streamItem.ItemId
                                            };
        }
    }

    //public class StreamItem_DefaultForUser : AbstractIndexCreationTask<StreamItem>
    //{
    //    public StreamItem_WithStuff()
    //    {
    //        Map = streamItems => from streamItem in streamItems

    //                             select new
    //                             {
    //                                 streamItem.ParentId,
    //                                 UserId = streamItem.User.Id,
    //                                 streamItem.CreatedDateTime,
    //                                 streamItem.Type,
    //                                 streamItem.ItemId
    //                             };

    //        TransformResults = (database, results) =>
    //                           from result in results
    //                           let observation = database.Load<Observation>(result.ItemId)
    //                           let post = database.Load<Post>(result.ItemId)
    //                           let observationNote = database.Load<ObservationNote>(result.ItemId)
    //                           select new
    //                            {
    //                                Item = post ?? observation ?? observationNote,

    //                            };
    //    }
    //}
}