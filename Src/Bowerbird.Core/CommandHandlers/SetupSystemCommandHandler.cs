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
using Bowerbird.Core.DomainModels.Members;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class SetupSystemCommandHandler : ICommandHandler<SetupSystemCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SetupSystemCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        private List<Permission> Permissions { get; set; }
        
        private List<Role> Roles { get; set; }

        private List<User> Users { get; set; }

        private List<Organisation> Organisations { get; set; }

        private List<Team> Teams { get; set; }

        private List<Project> Projects { get; set; }

        private List<GroupMember> GroupMembers { get; set; }

        private List<Observation> Observations { get; set; }

        private List<Post> Posts { get; set; }

        #endregion

        #region Methods

        public void Handle(SetupSystemCommand setupSystemCommand)
        {
            Check.RequireNotNull(setupSystemCommand, "setupSystemCommand");

            Permissions = new List<Permission>();
            Roles = new List<Role>();
            Users = new List<User>();
            Organisations = new List<Organisation>();
            Teams = new List<Team>();
            Projects = new List<Project>();
            GroupMembers = new List<GroupMember>();
            Observations = new List<Observation>();
            Posts = new List<Post>();

            try
            {
                // Permmissions
                AddPermission("editorganisations", "Edit Organisations", "Ability to create, update and delete organisations");
                AddPermission("editteams", "Edit Teams", "Ability to create, update and delete teams");
                AddPermission("editprojects", "Edit Projects", "Ability to create, update and delete projects");
                AddPermission("edituserobservations", "Edit Observations", "Ability to create, update and delete own observations");
                AddPermission("editteamobservations", "Edit Team Observations", "Ability to create, update and delete team observations");
                AddPermission("editprojectobservations", "Edit Project Observations", "Ability to create, update and delete project observations");

                // Roles
                AddRole("globaladministrator", "Global Administrator", "Administrator across entire system.","editorganisations");
                AddRole("globalmember", "Global Member", "Member of the system.", "global","edituserobservations");
                AddRole("organisationadministrator", "Organisation Administrator", "Administrator of an organisation.","editteams");
                AddRole("teamadministrator", "Team Administrator", "Administrator of a team.","editprojects");
                AddRole("teammember", "Team Member", "Member of a team.","editteamobservations");
                AddRole("projectadministrator", "Project Administrator", "Administrator of a project.");
                AddRole("projectmember", "Project Member", "Member of a project.","editprojectobservations");

                // Users
                AddUser("password", "frank@radocaj.com", "Frank", "Radocaj", "globaladministrator", "globalmember");
                AddUser("password", "hcrittenden@museum.vic.gov.au", "Hamish", "Crittenden", "globaladministrator", "globalmember");
                AddUser("password", "kwalker@museum.vic.gov.au", "Ken", "Walker","globaladministrator", "globalmember");

                // Organisations
                AddOrganisation("Bowerbird Test Organisation", "Test for Alpha Rlease", "www.bowerbird.org.au", Users[0].Id);
                AddOrganisation("Museum Victoria", "Museum Victoria", "www.museumvictoria.com", Users[0].Id);

                // Teams
                AddTeam("Bowerbird Test Team", "Test team for Alpha Release", "www.bowerbird.org.au", Users[0].Id, Organisations[0].Id);
                AddTeam("Ken Walker tests Bowerbird", "Another Test team for Alpha Release", "www.bowerbird.org.au", Users[2].Id, Organisations[1].Id);

                // Projects
                AddProject("Dev Alpha", "Test for Alpha Release", "www.bowerbird.org.au", Users[0].Id, Teams[0].Id);
                AddProject("Kens Bees", "Bee Project", "www.bowerbird.org.au", Users[2].Id, Teams[1].Id);

                AddProjectMember(Users[0].Id, Projects[0].Id, "projectmember");
                AddProjectMember(Users[0].Id, Projects[1].Id, "projectmember");
                AddProjectMember(Users[1].Id, Projects[0].Id, "projectmember");
                AddProjectMember(Users[1].Id, Projects[1].Id, "projectmember");
                AddProjectMember(Users[2].Id, Projects[0].Id, "projectmember");
                AddProjectMember(Users[2].Id, Projects[1].Id, "projectmember");

                AddTeamMember(Users[0].Id, Teams[0].Id, "teammember");
                AddTeamMember(Users[0].Id, Teams[1].Id, "teammember");
                AddTeamMember(Users[1].Id, Teams[0].Id, "teammember");
                AddTeamMember(Users[1].Id, Teams[1].Id, "teammember");
                AddTeamMember(Users[2].Id, Teams[0].Id, "teammember");
                AddTeamMember(Users[2].Id, Teams[1].Id, "teammember");

                AddObservation(Users[0].Id, Projects[0].Id);
                AddObservation(Users[0].Id, Projects[1].Id);
                AddObservation(Users[0].Id, Projects[0].Id);
                AddObservation(Users[0].Id, Projects[1].Id);
                AddObservation(Users[0].Id, Projects[1].Id);
                AddObservation(Users[1].Id, Projects[0].Id);
                AddObservation(Users[1].Id, Projects[1].Id);
                AddObservation(Users[1].Id, Projects[1].Id);
                AddObservation(Users[1].Id, Projects[0].Id);
                AddObservation(Users[1].Id, Projects[1].Id);
                AddObservation(Users[2].Id, Projects[1].Id);
                AddObservation(Users[2].Id, Projects[0].Id);
                AddObservation(Users[2].Id, Projects[0].Id);
            }
            finally
            {
                Permissions = null;
                Roles = null;
                Users = null;
                Organisations = null;
                Teams = null;
                Projects = null;
                GroupMembers = null;
                Observations = null;
                Posts = null;
            }
        }

        private void AddPermission(string id, string name, string description)
        {
            var permission = new Permission(id, name, description);

            _documentSession.Store(permission);

            Permissions.Add(permission);
        }

        private void AddRole(string id, string name, string description, params string[] permissionIds)
        {
            var permissions = Permissions.Where(x => permissionIds.Any(y => x.Id == "permissions/" + y));

            var role = new Role(id, name, description, permissions);

            _documentSession.Store(role);

            Roles.Add(role);
        }

        private void AddUser(string password, string email, string firstname, string lastname, params string[] roleIds)
        {
            var roles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));

            var user = new User(password, email, firstname, lastname, roles);

            _documentSession.Store(user);

            Users.Add(user);
        }

        private void AddOrganisation(string name, string description, string website, string userid)
        {
            var user = Users.Where(x => x.Id == userid).FirstOrDefault();

            Check.Ensure(user != null, "user may not be null");

            var organisation = new Organisation(user, name, description, website, null);

            _documentSession.Store(organisation);

            Organisations.Add(organisation);
        }

        private void AddTeam(string name, string description, string website, string userid, string organisationId = null)
        {
            var user = Users.Where(x => x.Id == userid).FirstOrDefault();

            Check.Ensure(user != null, "user may not be null");

            var team = new Team(user, name, description, website, null, organisationId);

            _documentSession.Store(team);

            Teams.Add(team);
        }

        private void AddProject(string name, string description, string website, string userid, string teamId = null)
        {
            var user = Users.Where(x => x.Id == userid).FirstOrDefault();

            Check.Ensure(user != null, "user may not be null");

            var project = new Project(user, name, description, website, null, teamId);

            _documentSession.Store(project);

            var team = Teams.Single(x => x.Id == teamId);

            team.AddGroupAssociation(project, user, DateTime.Now);

            _documentSession.Store(team);

            Projects.Add(project);
        }

        private void AddProjectMember(string userid, string projectId, string rolename)
        {
            var user = Users.Where(x => x.Id == userid).FirstOrDefault();
            var project = Projects.Where(x => x.Id == projectId).FirstOrDefault();
            var roles = new List<Role>() { Roles.Where(x => x.Id == "roles/" + rolename).FirstOrDefault() };

            Check.Ensure(user != null, "user may not be null");
            Check.Ensure(project != null, "project may not be null");
            Check.Ensure(roles.Count > 0, "role does not exist");

            var projectMember = new GroupMember(user, project, user, roles);

            _documentSession.Store(projectMember);

            GroupMembers.Add(projectMember);
        }

        private void AddTeamMember(string userid, string teamId, string rolename)
        {
            var user = Users.Where(x => x.Id == userid).FirstOrDefault();
            var team = Teams.Where(x => x.Id == teamId).FirstOrDefault();
            var roles = new List<Role>() { Roles.Where(x => x.Id == "roles/"+rolename).FirstOrDefault() };

            Check.Ensure(user != null, "user may not be null");
            Check.Ensure(team != null, "team may not be null");
            Check.Ensure(roles.Count > 0, "role does not exist");

            var teamMember = new GroupMember(user, team, user, roles);

            _documentSession.Store(teamMember);

            GroupMembers.Add(teamMember);
        }

        private void AddObservation(string userId, string projectId)
        {
            var user = Users.Where(x => x.Id == userId).FirstOrDefault();
            var project = Projects.Where(x => x.Id == projectId).FirstOrDefault();

            Check.Ensure(user != null, "user may not be null");
            Check.Ensure(project != null, "project may not be null");

            var observation = new Observation(
                user,
                "Title goes here",
                DateTime.Now,
                DateTime.Now,
                "23.232323",
                "41.3432423",
                "1 Main St Melbourne",
                true,
                "categoryX",
                new Dictionary<MediaResource, string>()
                );

            observation.AddGroupContribution(project, user, DateTime.Now);

            _documentSession.Store(observation);

            Observations.Add(observation);
        }

        #endregion      
      
    }
}
