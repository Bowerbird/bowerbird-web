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

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels.MediaResources;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ImageMediaResourceUpdateCommandHandler : ICommandHandler<ImageMediaResourceUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ImageMediaResourceUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ImageMediaResourceUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var imageMediaResource = _documentSession.Load<ImageMediaResource>(command.Id);

            imageMediaResource.UpdateDetails(command.Description);

            _documentSession.Store(imageMediaResource);
        }

        #endregion
    }
}