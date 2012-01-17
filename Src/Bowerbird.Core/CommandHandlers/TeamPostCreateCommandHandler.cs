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

    using System.Linq;

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.DomainModels.Posts;

    #endregion

    public class TeamPostCreateCommandHandler : ICommandHandler<TeamPostCreateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<TeamPost> _teamPostRepository;
        private readonly IRepository<MediaResource> _mediaResourceRepository;

        #endregion

        #region Constructors

        public TeamPostCreateCommandHandler(
            IRepository<User> userRepository
            , IRepository<Team> teamRepository
            , IRepository<TeamPost> teamPostRepository
            , IRepository<MediaResource> mediaResourceRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(teamRepository, "teamRepository");
            Check.RequireNotNull(teamPostRepository, "teamPostRepository");
            Check.RequireNotNull(mediaResourceRepository, "mediaResourceRepository");

            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _teamPostRepository = teamPostRepository;
            _mediaResourceRepository = mediaResourceRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamPostCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var teamPost = new TeamPost(
                _teamRepository.Load(command.TeamId),
                _userRepository.Load(command.UserId),
                command.PostedOn,
                command.Subject,
                command.Message,
                _mediaResourceRepository.Load(command.MediaResources).ToList());

            _teamPostRepository.Add(teamPost);
        }

        #endregion

    }
}