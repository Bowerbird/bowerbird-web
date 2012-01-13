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

    public class ImageMediaResourceCreateCommandHandler : ICommandHandler<ImageMediaResourceCreateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ImageMediaResource> _imageMediaResourceRepository;

        #endregion

        #region Constructors

        public ImageMediaResourceCreateCommandHandler(
            IRepository<User> userRepository
            ,IRepository<ImageMediaResource> imageMediaResourceRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(imageMediaResourceRepository, "mediaResourceRepository");

            _userRepository = userRepository;
            _imageMediaResourceRepository = imageMediaResourceRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ImageMediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var imageMediaResource = new ImageMediaResource(
                _userRepository.Load(command.UserId)
                , command.UploadedOn
                , command.OriginalFileName
                , command.FileFormat
                , command.Description
                , command.OriginalHeight
                , command.OriginalWidth);

            _imageMediaResourceRepository.Add(imageMediaResource);
        }

        #endregion
    }
}