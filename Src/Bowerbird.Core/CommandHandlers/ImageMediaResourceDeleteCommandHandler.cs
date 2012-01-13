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

    public class ImageMediaResourceDeleteCommandHandler : ICommandHandler<ImageMediaResourceDeleteCommand>
    {
        #region Fields

        private readonly IRepository<ImageMediaResource> _imageMediaResourceRepository;

        #endregion

        #region Constructors

        public ImageMediaResourceDeleteCommandHandler(
            IRepository<ImageMediaResource> imageMediaResourceRepository
            )
        {
            Check.RequireNotNull(imageMediaResourceRepository, "mediaResourceRepository");

            _imageMediaResourceRepository = imageMediaResourceRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ImageMediaResourceDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            _imageMediaResourceRepository.Remove(_imageMediaResourceRepository.Load(command.Id));
        }

        #endregion
    }
}