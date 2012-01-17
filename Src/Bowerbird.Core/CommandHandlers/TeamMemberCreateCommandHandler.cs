/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    public class TeamMemberCreateCommandHandler : ICommandHandler<TeamMemberCreateCommand>
    {
        #region Fields

        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<TeamMember> _teamMemberRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;

        #endregion

        #region Constructors

        public TeamMemberCreateCommandHandler(
            IRepository<Team> teamRepository
            ,IRepository<TeamMember> teamMemberRepository
            ,IRepository<User> userRepository
            ,IRepository<Role> roleRepository
            )
        {
            Check.RequireNotNull(teamRepository, "teamRepository");
            Check.RequireNotNull(teamMemberRepository, "teamMemberRepository");
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(roleRepository, "roleRepository");

            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamMemberCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var teamMember = new TeamMember(
                _userRepository.Load(command.CreatedByUserId),
                _teamRepository.Load(command.TeamId),
                _userRepository.Load(command.UserId),
                _roleRepository.Load(command.Roles)
                );

            _teamMemberRepository.Add(teamMember);
        }

        #endregion

    }
}