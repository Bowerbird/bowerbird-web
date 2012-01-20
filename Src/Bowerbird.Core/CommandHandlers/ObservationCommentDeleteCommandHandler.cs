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

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.Comments;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationCommentDeleteCommandHandler : ICommandHandler<ObservationCommentDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationCommentDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationCommentDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            _documentSession.Delete(_documentSession.Load<ObservationComment>(command.Id));
        }

        #endregion

    }
}