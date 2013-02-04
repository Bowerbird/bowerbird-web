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
using Bowerbird.Core.DomainModelFactories;
using Raven.Client;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdateCommandHandler : ICommandHandler<UserUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;

        #endregion

        #region Constructors

        public UserUpdateCommandHandler(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
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
                string.IsNullOrWhiteSpace(command.AvatarId) ? _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.User) : _documentSession.Load<MediaResource>(command.AvatarId),
                command.DefaultLicence,
                command.Timezone);

            var userProject = _documentSession.Load<UserProject>(user.Memberships.Single(x => x.Group.GroupType == "userproject").Group.Id);

            userProject.UpdateDetails(
                user,
                command.Name,
                command.Description,
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.User) : _documentSession.Load<MediaResource>(command.AvatarId),
                string.IsNullOrWhiteSpace(command.BackgroundId) ? _mediaResourceFactory.MakeDefaultBackgroundImage("userproject") : _documentSession.Load<MediaResource>(command.BackgroundId));

            _documentSession.Store(user);
            _documentSession.Store(userProject);
            _documentSession.SaveChanges();
        }

        #endregion      
    }
}