using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdatePasswordCommandHandler : ICommandHandler<UserUpdatePasswordCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserUpdatePasswordCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserUpdatePasswordCommand userUpdatePasswordCommand)
        {
            Check.RequireNotNull(userUpdatePasswordCommand, "userUpdatePasswordCommand");

            User user = null;

            if (!string.IsNullOrWhiteSpace(userUpdatePasswordCommand.ResetPasswordKey))
            {
                user = _documentSession.LoadUserByResetPasswordKey(userUpdatePasswordCommand.ResetPasswordKey);
            }

            if(!string.IsNullOrWhiteSpace(userUpdatePasswordCommand.UserId))
            {
                user = _documentSession.Load<User>(userUpdatePasswordCommand.UserId);
            }

            user.UpdatePassword(userUpdatePasswordCommand.Password);

            _documentSession.Store(user);
        }

        #endregion      
    }
}
