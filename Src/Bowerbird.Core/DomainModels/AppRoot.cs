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

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class AppRoot : Group
    {
        #region Members

        #endregion

        #region Constructors

        public AppRoot()
            : base() // Even though we are subclassing Group, we won't call the same constructor as other classes as this object won't have CreatedByUser available on the very first creation of the AppRoot.
        {
            InitMembers();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        private void InitMembers()
        {
            Id = Constants.AppRootId;
            SetDetails("Application Root Group");
        }

        public AppRoot SetUser(User createdByUser)
        {
            User = createdByUser;
            return this;
        }

        #endregion
    }
}