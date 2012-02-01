/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class StreamItem
    {
        #region Members

        #endregion

        #region Constructors

        protected StreamItem()
            : base()
        {
        }

        public StreamItem(
            User createdByUser
            , DateTime createdDateTime
            , string type
            , string id
            , DomainModel item
            , string parentId = null
            )
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            User = createdByUser;
            CreatedDateTime = createdDateTime;
            Type = type;
            ItemId = id;
            Item = item;
            ParentId = parentId;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string Type { get; set; }

        public string ItemId { get; set; }

        public string ParentId { get; set; }

        public DomainModel Item { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}