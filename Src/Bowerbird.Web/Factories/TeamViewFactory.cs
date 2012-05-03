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
using Bowerbird.Core.Paging;

namespace Bowerbird.Web.Factories
{
    public class TeamViewFactory : ITeamViewFactory
    {
        #region Fields

        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public TeamViewFactory(
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

        public object Make(Team team)
        {
            return new
            {
                team.Id,
                team.Name,
                team.Description,
                team.Website,
                Avatar = _avatarFactory.GetAvatar(team)
            };
        }

        public object Make(Team team, PagedList<object> projects, object organisation, PagedList<object> members)
        {
            return new
            {
                team.Id,
                team.Name,
                team.Description,
                team.Website,
                Avatar = _avatarFactory.GetAvatar(team),
                Projects = projects,
                Organisation = organisation,
                Members = members
            };
        }

        #endregion
    }
}


