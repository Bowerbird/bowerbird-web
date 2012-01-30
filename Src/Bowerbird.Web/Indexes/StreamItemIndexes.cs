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
    public class StreamItem_ByParentId : AbstractIndexCreationTask<StreamItem>
    {
        public StreamItem_ByParentId()
        {
            Map = streamItems => streamItems.Select(streamItem => streamItem.ParentId);
        }
    }

    public class StreamItem_ByUserId : AbstractIndexCreationTask<StreamItem>
    {
        public StreamItem_ByUserId()
        {
            Map = streamItems => streamItems.Select(streamItem => streamItem.User.Id);
        }
    }

    public class StreamItem_ByType : AbstractIndexCreationTask<StreamItem>
    {
        public StreamItem_ByType()
        {
            Map = streamItems => streamItems.Select(streamItem => streamItem.Type);
        }
    }
}