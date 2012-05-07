/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class FollowUserCreateCommandHandler : ICommandHandler<FollowUserCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public FollowUserCreateCommandHandler(
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion

        public void Handle(FollowUserCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var followUser = new FollowUser(
                    _documentSession.Load<User>(command.UserToFollow),
                    _documentSession.Load<User>(command.Follower),
                    command.CreatedDateTime
                );

            _documentSession.Store(followUser);
        }
    }
}