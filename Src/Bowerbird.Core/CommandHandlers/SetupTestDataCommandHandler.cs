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
using Bowerbird.Core.Events;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.CommandHandlers
{
    public class SetupTestDataCommandHandler : ICommandHandler<SetupTestDataCommand>
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly ISystemStateManager _systemStateManager;

        #endregion

        #region Constructors

        public SetupTestDataCommandHandler(
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

        public void Handle(SetupTestDataCommand setupTestDataCommand)
        {
            Check.RequireNotNull(setupTestDataCommand, "setupTestDataCommand");

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
                _systemStateManager.DisableEmailService();

                Roles = _documentSession.Query<Role>().ToList();
                Users = _documentSession.Query<User>().ToList();

                // Users
                //AddUser("password", "frank@radocaj.com", "Frank", "Radocaj", "globaladministrator", "globalmember");

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
                Users = null;
                Organisations = null;
                Teams = null;
                Projects = null;
                GroupMembers = null;
                Observations = null;
                Posts = null;

                _systemStateManager.EnableEmailService();
            }
        }

        private void AddUser(string password, string email, string firstname, string lastname, params string[] roleIds)
        {
            var globalRoles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));
            var user = new User(password, email, firstname, lastname, globalRoles);
            _documentSession.Store(user);
            Users.Add(user);

            var userProject = new UserProject(user);
            _documentSession.Store(userProject);

            var userProjectRoles = Roles.Where(x => x.Id == "roles/projectadministrator" || x.Id == "roles/projectmember");
            var groupMember = new GroupMember(user, userProject, user, userProjectRoles);
            _documentSession.Store(groupMember);
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
                "categoryX");

            observation.AddGroupContribution(project, user, DateTime.Now);

            _documentSession.Store(observation);

            Observations.Add(observation);
        }

        #endregion      
      
    }
}
