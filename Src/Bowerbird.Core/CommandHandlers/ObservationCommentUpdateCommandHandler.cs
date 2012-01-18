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

using Bowerbird.Core.DomainModels.Comments;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class ObservationCommentUpdateCommandHandler : ICommandHandler<ObservationCommentUpdateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationCommentUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationCommentUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var observationComment = _documentSession
                .Load<ObservationComment>(command.Id)
                .UpdateCommentMessage(
                    _documentSession.Load<User>(command.UserId),
                    command.UpdatedOn,
                    command.Comment);

            _documentSession.Store(observationComment);
        }

        #endregion

    }
}