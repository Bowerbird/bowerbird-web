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
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Factories
{
    public class ProjectViewFactory : IProjectViewFactory
    {
        #region Fields

        //private readonly IUserViewFactory _userViewFactory;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ProjectViewFactory(
            IAvatarFactory avatarFactory
            )
        {
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// Return the Project properties and avatar
        /// passing the Project Domain Model
        /// </summary>
        public object Make(Project project)
        {
            return new
            {
                project.Id,
                project.Name,
                project.Description,
                project.Website,
                Avatar = _avatarFactory.Make(project)
            };
        }

        /// <summary>
        /// Return the Project properties and avatar with
        /// project member count
        /// passing the index All_Groups.Result
        /// </summary>
        public object Make(All_Groups.Result project)
        {
            return new
            {
                project.Id,
                project.Project.Name,
                project.Project.Description,
                project.Project.Website,
                Avatar = _avatarFactory.Make(project.Project),
                project.GroupMemberCount
            };
        }

        #endregion
    }
}