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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdateCommandHandler : ICommandHandler<UserUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var user = _documentSession.Load<User>(command.Id);

            user.UpdateDetails(
                command.FirstName,
                command.LastName,
                command.Description,
                string.IsNullOrWhiteSpace(command.AvatarId) ? null : _documentSession.Load<MediaResource>(command.AvatarId));

            _documentSession.Store(user);
        }

        #endregion      
    }
}