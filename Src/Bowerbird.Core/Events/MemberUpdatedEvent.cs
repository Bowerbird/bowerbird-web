/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class MemberUpdatedEvent : DomainModelUpdatedEvent<Member>
    {

        #region Members

        #endregion

        #region Constructors

        public MemberUpdatedEvent(
            Member domainModel,
            User createdByUser, 
            DomainModel sender,
            Group group)
            : base(
            domainModel,
            createdByUser,
            sender)
        {
            Group = group;
        }

        #endregion

        #region Properties

        public Group Group { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}