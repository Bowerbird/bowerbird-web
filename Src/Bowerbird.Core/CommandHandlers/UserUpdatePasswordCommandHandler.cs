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

using System.Linq;
using Bowerbird.Core.Commands;
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

        public void Handle(UserUpdatePasswordCommand command)
        {
            Check.RequireNotNull(command, "command");

            User user = null;

            if (!string.IsNullOrWhiteSpace(command.ResetPasswordKey))
            {
                user = _documentSession
                    .Query<User>()
                    .Where(x => x.ResetPasswordKey == command.ResetPasswordKey)
                    .FirstOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(command.UserId))
            {
                user = _documentSession.Load<User>(command.UserId);
            }

            user.UpdatePassword(command.Password);

            _documentSession.Store(user);
            _documentSession.SaveChanges();
        }

        #endregion      
    }
}
