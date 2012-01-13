/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Posts;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class TeamPostCreateCommandHandler : ICommandHandler<TeamPostCreateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<TeamPost> _teamPostRepository;

        #endregion

        #region Constructors

        public TeamPostCreateCommandHandler(
            IRepository<User> userRepository
            , IRepository<Team> teamRepository
            , IRepository<TeamPost> teamPostRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(teamRepository, "teamRepository");
            Check.RequireNotNull(teamPostRepository, "teamPostRepository");

            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _teamPostRepository = teamPostRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamPostCreateCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}