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
using Raven.Client;
using System;
using System.Linq;
using Bowerbird.Core.Config;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserCreateCommandHandler : ICommandHandler<UserCreateCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserCreateCommand userCreateCommand)
        {
            Check.RequireNotNull(userCreateCommand, "userCreateCommand");

            var user = new User(
                userCreateCommand.Password,
                userCreateCommand.Email,
                userCreateCommand.FirstName,
                userCreateCommand.LastName);
            _documentSession.Store(user);

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);
            var roles = _documentSession.Query<Role>().Where(x => x.Id.In(userCreateCommand.Roles));

            var member = new Member(
                user, 
                user,
                appRoot, 
                roles);
            _documentSession.Store(member);

            user.AddMembership(member);
            _documentSession.Store(user);

            var userProject = new UserProject(user, DateTime.Now);
            userProject.SetAncestry(appRoot);
            _documentSession.Store(userProject);

            var userProjectAssociation = new GroupAssociation(appRoot, userProject, user, DateTime.Now);
            _documentSession.Store(userProjectAssociation);

            var userProjectRoles = _documentSession.Query<Role>().Where(x => x.Id == "roles/projectadministrator" || x.Id == "roles/projectmember");
            var userProjectMember = new Member(user, user, userProject, userProjectRoles);
            _documentSession.Store(userProjectMember);
        }

        #endregion      
      
    }
}
