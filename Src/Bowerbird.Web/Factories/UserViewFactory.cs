/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using System.Linq;

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

            return new
            {
                user.Id,
                user.Avatar,
                user.FirstName,
                user.LastName,
                Name = user.GetName(),
                user.SessionLatestActivity
            };
        }

        #endregion   
    }
}
