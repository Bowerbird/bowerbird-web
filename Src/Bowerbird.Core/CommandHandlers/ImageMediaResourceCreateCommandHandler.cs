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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels.MediaResources;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ImageMediaResourceCreateCommandHandler : ICommandHandler<ImageMediaResourceCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ImageMediaResourceCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ImageMediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var imageMediaResource = new ImageMediaResource(
                _documentSession.Load<User>(command.UserId)
                , command.UploadedOn
                , command.OriginalFileName
                , command.FileFormat
                , command.Description
                , command.OriginalHeight
                , command.OriginalWidth);

            _documentSession.Store(imageMediaResource);
        }

        #endregion
    }
}