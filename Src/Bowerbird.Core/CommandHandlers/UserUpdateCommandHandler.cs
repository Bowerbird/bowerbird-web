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
using Bowerbird.Core.Extensions;
using Raven.Client;
using Bowerbird.Core.Config;

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
                command.Name,
                command.Description,
                string.IsNullOrWhiteSpace(command.AvatarId) ? null : _documentSession.Load<MediaResource>(command.AvatarId),
                command.DefaultLicence,
                command.Timezone);

            var userProject = _documentSession.Load<UserProject>(user.Memberships.Single(x => x.Group.GroupType == "userproject").Group.Id);

            userProject.UpdateDetails(
                user,
                command.Name,
                command.Description,
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? null : _documentSession.Load<MediaResource>(command.AvatarId),
                string.IsNullOrWhiteSpace(command.BackgroundId) ? null : _documentSession.Load<MediaResource>(command.BackgroundId));

            _documentSession.Store(user);
            _documentSession.Store(userProject);
        }

        #endregion      
    }
}