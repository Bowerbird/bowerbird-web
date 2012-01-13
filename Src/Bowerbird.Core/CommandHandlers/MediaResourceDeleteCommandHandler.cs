/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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

    public class MediaResourceDeleteCommandHandler : ICommandHandler<MediaResourceDeleteCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MediaResource> _mediaResourceRepository;

        #endregion

        #region Constructors

        public MediaResourceDeleteCommandHandler(
             IRepository<User> userRepository
            , IRepository<MediaResource> mediaResourceRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(mediaResourceRepository, "mediaResourceRepository");

            _userRepository = userRepository;
            _mediaResourceRepository = mediaResourceRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MediaResourceDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}