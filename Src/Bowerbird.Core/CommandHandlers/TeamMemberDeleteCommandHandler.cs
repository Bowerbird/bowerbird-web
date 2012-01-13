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

    public class TeamMemberDeleteCommandHandler : ICommandHandler<TeamMemberDeleteCommand>
    {
        #region Fields

        private readonly IRepository<TeamMember> _teamMemberRepository;

        #endregion

        #region Constructors

        public TeamMemberDeleteCommandHandler(
            IRepository<TeamMember> teamMemberRepository
            )
        {
            Check.RequireNotNull(teamMemberRepository, "teamMemberRepository");

            _teamMemberRepository = teamMemberRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamMemberDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}