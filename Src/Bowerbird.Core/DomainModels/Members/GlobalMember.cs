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

using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels.Members
{
    public class GlobalMember : Member
    {
        #region Members

        #endregion

        #region Constructors

        protected GlobalMember()
            : base()
        {
            InitMembers();
        }

        public GlobalMember(
            User user,
            IEnumerable<Role> roles)
            : base(
            user,
            roles)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        private new void SetDetails(User user)
        {
            base.SetDetails(user);
        }

        #endregion
    }
}