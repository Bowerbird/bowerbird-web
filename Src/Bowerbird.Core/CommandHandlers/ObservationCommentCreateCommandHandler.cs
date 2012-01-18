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

using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.DomainModels.Comments;

    #endregion

    public class ObservationCommentCreateCommandHandler : ICommandHandler<ObservationCommentCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationCommentCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationCommentCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var observationComment = new ObservationComment(
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<Observation>(command.ObservationId),
                command.CommentedOn,
                command.Comment
                );

            _documentSession.Store(observationComment);
        }

        #endregion

    }
}