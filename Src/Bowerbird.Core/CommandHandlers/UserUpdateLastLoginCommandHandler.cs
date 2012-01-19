using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdateLastLoginCommandHandler : ICommandHandler<UserUpdateLastLoginCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserUpdateLastLoginCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserUpdateLastLoginCommand userUpdateLastLoginCommand)
        {
            Check.RequireNotNull(userUpdateLastLoginCommand, "userUpdateLastLoginCommand");

            var user = _documentSession.LoadUserByEmail(userUpdateLastLoginCommand.Email);

            user.UpdateLastLoggedIn();

            _documentSession.Store(user);
        }

        #endregion      
    }
}