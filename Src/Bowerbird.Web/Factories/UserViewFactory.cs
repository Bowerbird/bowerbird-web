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
using Bowerbird.Core.DomainModels;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;

namespace Bowerbird.Web.Factories
{
    public class UserViewFactory : IUserViewFactory
    {
        #region Fields

        // TODO: Inject rather than service locator...

        protected readonly IAvatarFactory _avatarFactory;
        protected readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserViewFactory()
        {
            _avatarFactory = ServiceLocator.Current.GetInstance<IAvatarFactory>();
            _documentSession = ServiceLocator.Current.GetInstance<IDocumentSession>();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(string id)
        {
            var user = _documentSession.Load<User>(id);

            return new
            {
                Avatar = _avatarFactory.GetAvatar(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName(),
            };
        }

        public object Make(User user)
        {
            Check.RequireNotNull(user, "user");

            return new
            {
                Avatar = _avatarFactory.GetAvatar(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName(),
            };
        }

        #endregion
    }
}