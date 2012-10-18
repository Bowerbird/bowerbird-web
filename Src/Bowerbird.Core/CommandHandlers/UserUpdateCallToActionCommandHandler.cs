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
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Repositories;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdateCallToActionCommandHandler : ICommandHandler<UserUpdateCallToActionCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserUpdateCallToActionCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserUpdateCallToActionCommand userUpdateCallToActionCommand)
        {
            Check.RequireNotNull(userUpdateCallToActionCommand, "userUpdateCallToActionCommand");

            var user = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == userUpdateCallToActionCommand.UserId)
                .First()
                .User;

            user.RemoveCallToAction(userUpdateCallToActionCommand.Name);

            _documentSession.Store(user);
        }

        #endregion      
    }
}