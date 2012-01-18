using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserRequestPasswordResetCommandHandler : ICommandHandler<UserRequestPasswordResetCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserRequestPasswordResetCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserRequestPasswordResetCommand userRequestPasswordResetCommand)
        {
            Check.RequireNotNull(userRequestPasswordResetCommand, "userRequestPasswordResetCommand");

            var user = _documentSession.LoadUserByEmail(userRequestPasswordResetCommand.Email);

            user.RequestPasswordReset();

            _documentSession.Store(user);
        }

        #endregion      
    }
}
