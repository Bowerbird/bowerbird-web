/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class UserViewFactory : IUserViewFactory
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(User user)
        {
            Check.RequireNotNull(user, "user");

            var userId = user.Id.MinifyId<User>();

            return new
            {
                Id = userId,
                user.Avatar,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        #endregion   
    }
}
