/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Members;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class TeamMemberUpdateCommandHandler : ICommandHandler<TeamMemberUpdateCommand>
    {
        #region Fields

        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<TeamMember> _teamMemberRepository;
        private readonly IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public TeamMemberUpdateCommandHandler(
             IRepository<Team> teamRepository
            , IRepository<TeamMember> teamMemberRepository
            , IRepository<User> userRepository
            )
        {
            Check.RequireNotNull(teamRepository, "teamRepository");
            Check.RequireNotNull(teamMemberRepository, "teamMemberRepository");
            Check.RequireNotNull(userRepository, "userRepository");

            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamMemberUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}