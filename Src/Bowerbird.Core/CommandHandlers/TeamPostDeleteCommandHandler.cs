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

    public class TeamPostDeleteCommandHandler : ICommandHandler<TeamPostDeleteCommand>
    {
        #region Fields

        private readonly IRepository<TeamPost> _teamPostRepository;

        #endregion

        #region Constructors

        public TeamPostDeleteCommandHandler(
            IRepository<TeamPost> teamPostRepository
            )
        {
            Check.RequireNotNull(teamPostRepository, "teamPostRepository");

            _teamPostRepository = teamPostRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamPostDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}