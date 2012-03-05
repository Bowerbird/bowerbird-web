/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels.Members
{
    public class GroupMember : Member
    {
        #region Members

        #endregion

        #region Constructors

        protected GroupMember()
            : base()
        {
        }

        public GroupMember(
            User createdByUser,
            Group group,
            User user,
            IEnumerable<Role> roles)
            : base(
            user,
            roles)
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(createdByUser, "createdByUser");

            Group = group;

            var eventMessage = string.Format(
                ActivityMessage.AddMemberToGroup,
                user.GetName(),
                group.Name,
                group.GroupType()
                );

            EventProcessor.Raise(new DomainModelCreatedEvent<GroupMember>(this, createdByUser, eventMessage));
        }

        #endregion

        #region Properties

        public DenormalisedNamedDomainModelReference<Group> Group { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}