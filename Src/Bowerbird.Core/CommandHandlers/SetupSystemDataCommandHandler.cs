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

using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.Events;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.CommandHandlers
{
    public class SetupSystemDataCommandHandler : ICommandHandler<SetupSystemDataCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly ISystemStateManager _systemStateManager;

        #endregion

        #region Constructors

        public SetupSystemDataCommandHandler(
            IDocumentSession documentSession,
            ISystemStateManager systemStateManager)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(systemStateManager, "systemStateManager");

            _documentSession = documentSession;
            _systemStateManager = systemStateManager;
        }

        #endregion

        #region Properties

        private AppRoot TheAppRoot { get; set; }

        private List<Permission> Permissions2 { get; set; }
        
        private List<Role> Roles { get; set; }

        private List<User> Users { get; set; }

        #endregion

        #region Methods

        public void Handle(SetupSystemDataCommand setupSystemDataCommand)
        {
            Check.RequireNotNull(setupSystemDataCommand, "setupSystemDataCommand");

            TheAppRoot = null;
            Permissions2 = new List<Permission>();
            Roles = new List<Role>();
            Users = new List<User>();

            try
            {
                _systemStateManager.DisableEmailService();

                AddAppRoot();

                // Permmissions
                AddPermission(PermissionNames.CreateOrganisation, "Create Organisations", "Ability to create organisations");
                AddPermission(PermissionNames.UpdateOrganisation, "Update Organisations", "Ability to update organisations");
                AddPermission(PermissionNames.DeleteOrganisation, "Delete Organisations", "Ability to delete organisations");
                AddPermission(PermissionNames.CreateTeam, "Create Teams", "Ability to create teams");
                AddPermission(PermissionNames.UpdateTeam, "Update Teams", "Ability to update teams");
                AddPermission(PermissionNames.DeleteTeam, "Delete Teams", "Ability to delete teams");
                AddPermission(PermissionNames.CreateProject, "Create Projects", "Ability to create projects");
                AddPermission(PermissionNames.UpdateProject, "Update Projects", "Ability to update projects");
                AddPermission(PermissionNames.DeleteProject, "Delete Projects", "Ability to delete projects");
                AddPermission(PermissionNames.CreateWatchlist, "Create Watchlists", "Ability to create watchlists");
                AddPermission(PermissionNames.UpdateWatchlist, "Update Watchlists", "Ability to update watchlists");
                AddPermission(PermissionNames.DeleteWatchlist, "Delete Watchlists", "Ability to delete watchlists");
                AddPermission(PermissionNames.CreateObservation, "Create Observations", "Ability to create observations");
                AddPermission(PermissionNames.UpdateObservation, "Update Observations", "Ability to update observations");
                AddPermission(PermissionNames.DeleteObservation, "Delete Observations", "Ability to delete observations");
                AddPermission(PermissionNames.CreatePost, "Create Posts", "Ability to create posts");
                AddPermission(PermissionNames.UpdatePost, "Update Posts", "Ability to update posts");
                AddPermission(PermissionNames.DeletePost, "Delete Posts", "Ability to delete posts");

                // Roles
                AddRole("globaladministrator", "Global Administrator", "Administrator of Bowerbird", 
                    PermissionNames.CreateOrganisation, 
                    PermissionNames.UpdateOrganisation, 
                    PermissionNames.DeleteOrganisation);
                AddRole("globalmember", "Global Member", "Member of Bowerbird", 
                    PermissionNames.CreateObservation, 
                    PermissionNames.UpdateObservation, 
                    PermissionNames.DeleteObservation, 
                    PermissionNames.CreateProject, 
                    PermissionNames.UpdateProject, 
                    PermissionNames.DeleteProject);
                AddRole("organisationadministrator", "Organisation Administrator", "Administrator of an organisation", 
                    PermissionNames.UpdateOrganisation, 
                    PermissionNames.CreateTeam, 
                    PermissionNames.UpdateTeam, 
                    PermissionNames.DeleteTeam,
                    PermissionNames.CreatePost,
                    PermissionNames.UpdatePost,
                    PermissionNames.DeletePost);
                AddRole("teamadministrator", "Team Administrator", "Administrator of a team", 
                    PermissionNames.UpdateTeam, 
                    PermissionNames.CreateProject, 
                    PermissionNames.UpdateProject, 
                    PermissionNames.DeleteProject);
                AddRole("teammember", "Team Member", "Member of a team",
                    PermissionNames.CreatePost,
                    PermissionNames.UpdatePost,
                    PermissionNames.DeletePost);
                AddRole("projectadministrator", "Project Administrator", "Administrator of a project",
                    PermissionNames.UpdateProject);
                AddRole("projectmember", "Project Member", "Member of a project", 
                    PermissionNames.CreateObservation, 
                    PermissionNames.UpdateObservation, 
                    PermissionNames.DeleteObservation,
                    PermissionNames.CreatePost,
                    PermissionNames.UpdatePost,
                    PermissionNames.DeletePost);

                // Admin Users
                AddUser("password", "frank@radocaj.com", "Frank", "Radocaj", "globaladministrator", "globalmember");
                UpdateAppRoot(Users[0].Id); // Set user on app root now that we have one
                AddUser("password", "hcrittenden@museum.vic.gov.au", "Hamish", "Crittenden", "globaladministrator", "globalmember");
                AddUser("password", "kwalker@museum.vic.gov.au", "Ken", "Walker","globaladministrator", "globalmember");

                _systemStateManager.SystemDataSetupDate(DateTime.UtcNow);
            }
            finally
            {
                TheAppRoot = null;
                Permissions2 = null;
                Roles = null;
                Users = null;

                _systemStateManager.EnableEmailService();
            }
        }

        private void AddAppRoot()
        {
            TheAppRoot = new AppRoot();
            _documentSession.Store(TheAppRoot);
        }

        private void UpdateAppRoot(string userId)
        {
            TheAppRoot.SetUser(Users.Single(x => x.Id == userId));
            _documentSession.Store(TheAppRoot);
        }

        private void AddPermission(string id, string name, string description)
        {
            var permission = new Permission(id, name, description);

            _documentSession.Store(permission);

            Permissions2.Add(permission);
        }

        private void AddRole(string id, string name, string description, params string[] permissionIds)
        {
            var permissions = Permissions2.Where(x => permissionIds.Any(y => x.Id == "permissions/" + y));

            var role = new Role(id, name, description, permissions);

            _documentSession.Store(role);

            Roles.Add(role);
        }

        private void AddUser(string password, string email, string firstname, string lastname, params string[] roleIds)
        {
            var user = new User(password, email, firstname, lastname);
            _documentSession.Store(user);

            var member = new Member(
                user,
                user,
                TheAppRoot,
                Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y)));
            _documentSession.Store(member);

            user.AddMembership(member);
            _documentSession.Store(user);

            var userProject = new UserProject(user, DateTime.Now);
            _documentSession.Store(userProject);

            var userProjectAssociation = new GroupAssociation(TheAppRoot, userProject, user, DateTime.Now);
            _documentSession.Store(userProjectAssociation);

            var userProjectmember = new Member(
                user, 
                user, 
                userProject, 
                Roles.Where(x => x.Id == "roles/projectadministrator" || x.Id == "roles/projectmember"));
            _documentSession.Store(userProjectmember);

            user.AddMembership(userProjectmember);
            _documentSession.Store(user);

            Users.Add(user);
        }

        #endregion      
      
    }
}
