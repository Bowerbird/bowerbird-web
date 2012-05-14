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

        /// <summary>
        /// Return the Team properties and avatar
        /// passing the Team Domain Model
        /// </summary>
        public object Make(Team team)
        {
            return new
            {
                team.Id,
                team.Name,
                team.Description,
                team.Website,
                Avatar = _avatarFactory.Make(team)
            };
        }

        /// <summary>
        /// Return the Team properties and avatar with
        /// project count and member count
        /// passing the index All_Groups.Result
        /// </summary>
        public object Make(All_Groups.Result team)
        {
            return new
            {
                team.Id,
                team.Team.Name,
                team.Team.Description,
                team.Team.Website,
                Avatar = _avatarFactory.Make(team.Team),
                team.GroupMemberCount,
                Projects = 0
            };
        }

        #endregion
    }
}