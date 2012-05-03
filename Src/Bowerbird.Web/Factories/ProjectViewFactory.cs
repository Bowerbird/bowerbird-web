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

namespace Bowerbird.Web.Factories
{
    public class ProjectViewFactory : IProjectViewFactory
    {
        #region Fields

        private readonly IUserViewFactory _userViewFactory;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ProjectViewFactory(
            IUserViewFactory userViewFactory,
            IAvatarFactory avatarFactory
            )
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _userViewFactory = userViewFactory;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(Project project)
        {
            return new
            {
                project.Id,
                project.Name,
                project.Description,
                project.Website,
                Creator = _userViewFactory.Make(project.User.Id),
                Avatar = _avatarFactory.GetAvatar(project)
            };
        }

        #endregion
    }
}