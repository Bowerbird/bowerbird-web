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
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class UserProject : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected UserProject()
            : base()
        {
        }

        public UserProject(
            User createdByUser,
            DateTime createdDateTime,
            Group parentGroup)
            : base(
            createdByUser,
            "User Group",
            createdDateTime,
            parentGroup)
        {
            ApplyEvent(new DomainModelCreatedEvent<UserProject>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public override string GroupType
        {
            get { return "userproject"; }
        }
        
        #endregion

        #region Methods

        #endregion
    }
}