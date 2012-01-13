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

    #endregion

    public class TeamDeleteCommandHandler : ICommandHandler<TeamDeleteCommand>
    {
        #region Fields

        private readonly IRepository<Team> _teamRepository;

        #endregion

        #region Constructors

        public TeamDeleteCommandHandler(
            IRepository<Team> teamRepository
            )
        {
            Check.RequireNotNull(teamRepository, "teamRepository");

            _teamRepository = teamRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}