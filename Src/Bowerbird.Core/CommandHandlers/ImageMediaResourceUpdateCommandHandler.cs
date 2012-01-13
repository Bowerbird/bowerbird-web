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

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DomainModels.MediaResources;

    #endregion

    public class ImageMediaResourceUpdateCommandHandler : ICommandHandler<ImageMediaResourceUpdateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ImageMediaResource> _imageMediaResourceRepository;

        #endregion

        #region Constructors

        public ImageMediaResourceUpdateCommandHandler(
            IRepository<User> userRepository
            , IRepository<ImageMediaResource> imageMediaResourceRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(imageMediaResourceRepository, "imageMediaResourceRepository");

            _userRepository = userRepository;
            _imageMediaResourceRepository = imageMediaResourceRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ImageMediaResourceUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var imageMediaResource = _imageMediaResourceRepository.Load(command.Id);

            imageMediaResource.UpdateDetails(command.Description);

            _imageMediaResourceRepository.Add(imageMediaResource);
        }

        #endregion
    }
}