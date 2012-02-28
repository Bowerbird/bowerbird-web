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
using Raven.Client;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceDeleteCommandHandler : ICommandHandler<MediaResourceDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MediaResourceDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MediaResourceDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            _documentSession.Delete(_documentSession.Load<MediaResource>(command.Id));
        }

        #endregion
    }
}