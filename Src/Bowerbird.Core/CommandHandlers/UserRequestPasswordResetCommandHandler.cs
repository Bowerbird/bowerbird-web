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

        public void Handle(UserRequestPasswordResetCommand command)
        {
            Check.RequireNotNull(command, "command");

            var user = _documentSession.Query<User>().Single(x => x.Email == command.Email);
            
            user.RequestPasswordReset();

            _documentSession.Store(user);
            _documentSession.SaveChanges();
        }

        #endregion      
    }
}
