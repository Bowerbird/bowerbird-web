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
using System.Threading;

namespace Bowerbird.Core.CommandHandlers
{
    public class SetupTestDataCommandHandler : ICommandHandler<SetupTestDataCommand>
    {

        #region Members

        private readonly IDocumentStore _documentStore;
        private readonly IDocumentSession _documentSession;
        private readonly ISystemStateManager _systemStateManager;

        #endregion

        #region Constructors

        public SetupTestDataCommandHandler(
            IDocumentStore documentStore,
            IDocumentSession documentSession,
            ISystemStateManager systemStateManager)
        {
            Check.RequireNotNull(documentStore, "documentStore");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(systemStateManager, "systemStateManager");

            _documentStore = documentStore;
            _documentSession = documentSession;
            _systemStateManager = systemStateManager;
        }

        #endregion

        #region Properties

        private AppRoot TheAppRoot { get; set; }

        private List<Role> Roles { get; set; }

        private List<User> Users { get; set; }

        private List<Organisation> Organisations { get; set; }

        private List<Team> Teams { get; set; }

        private List<Project> Projects { get; set; }

        private List<Member> Members { get; set; }

        private List<Observation> Observations { get; set; }

        private List<Post> Posts { get; set; }

        private List<GroupAssociation> GroupAssociations { get; set; }

        #endregion

        #region Methods

        public void Handle(SetupTestDataCommand setupTestDataCommand)
        {
            Check.RequireNotNull(setupTestDataCommand, "setupTestDataCommand");

            try
            {
                TheAppRoot = null;
                Organisations = new List<Organisation>();
                Teams = new List<Team>();
                Projects = new List<Project>();
                Members = new List<Member>();
                Observations = new List<Observation>();
                Posts = new List<Post>();
                GroupAssociations = new List<GroupAssociation>();
                Roles = _documentSession.Query<Role>().ToList();
                Users = _documentSession.Query<User>().ToList();

                // Disable emailing while we setup admin users
                _systemStateManager.DisableEmailService();

                TheAppRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

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

                // Observations
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

                // Save all data now
                _documentSession.SaveChanges();

                // Wait for all stale indexes to complete.
                while (_documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
                {
                    Thread.Sleep(10);
                }

                // Re-enable emailing
                _systemStateManager.EnableEmailService();
            }
            catch (Exception exception)
            {
                throw new Exception("Could not setup test data.", exception);
            }
        }

        //private void AddUser(string password, string email, string firstname, string lastname, params string[] roleIds)
        //{
        //    var globalRoles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));
        //    var user = new User(password, email, firstname, lastname, globalRoles);
        //    _documentSession.Store(user);
        //    Users.Add(user);

        //    var userProject = new UserProject(user);
        //    _documentSession.Store(userProject);

        //    var userProjectRoles = Roles.Where(x => x.Id == "roles/projectadministrator" || x.Id == "roles/projectmember");
        //    var groupMember = new GroupMember(user, userProject, user, userProjectRoles);
        //    _documentSession.Store(groupMember);
        //}

        private void AddOrganisation(string name, string description, string website, string userid)
        {
            var organisation = new Organisation(Users.Single(x => x.Id == userid), name, description, website, null, DateTime.Now);
            _documentSession.Store(organisation);

            var groupAssociation = new GroupAssociation(TheAppRoot, organisation, Users.Single(x => x.Id == userid), DateTime.Now);
            GroupAssociations.Add(groupAssociation);
            _documentSession.Store(groupAssociation);

            organisation.SetAncestry(TheAppRoot);
            _documentSession.Store(organisation);

            Organisations.Add(organisation);
        }

        private void AddTeam(string name, string description, string website, string userid, string organisationId = null)
        {
            var team = new Team(Users.Single(x => x.Id == userid), name, description, website, null, DateTime.Now);
            _documentSession.Store(team);

            var parentGroup = Organisations.Single(x => x.Id == organisationId);

            var groupAssociation = new GroupAssociation(parentGroup, team, Users.Single(x => x.Id == userid), DateTime.Now);
            GroupAssociations.Add(groupAssociation);
            _documentSession.Store(groupAssociation);

            team.SetAncestry(parentGroup);
            _documentSession.Store(team);

            parentGroup.AddDescendant(team);
            _documentSession.Store(parentGroup);

            Teams.Add(team);
        }

        private void AddProject(string name, string description, string website, string userid, string teamId = null)
        {
            var project = new Project(Users.Single(x => x.Id == userid), name, description, website, null, DateTime.Now);
            _documentSession.Store(project);

            var parentGroup = Teams.Single(x => x.Id == teamId);

            var groupAssociation = new GroupAssociation(parentGroup, project, Users.Single(x => x.Id == userid), DateTime.Now);
            GroupAssociations.Add(groupAssociation);
            _documentSession.Store(groupAssociation);

            project.SetAncestry(parentGroup);
            _documentSession.Store(project);

            parentGroup.AddDescendant(project);
            _documentSession.Store(parentGroup);
             
            // Update parent of parent (organisation) with descendant
            var parentGroupAssociation = GroupAssociations.Single(x => x.ChildGroupId == parentGroup.Id);
            var organisation = Organisations.Single(x => x.Id == parentGroupAssociation.ParentGroupId);
            organisation.AddDescendant(project);
            _documentSession.Store(organisation);

            Projects.Add(project);
        }

        private void AddProjectMember(string userid, string projectId, string rolename)
        {
            var user = Users.Single(x => x.Id == userid);
            var project = Projects.Single(x => x.Id == projectId);
            var roles = new List<Role>() { Roles.Single(x => x.Id == "roles/" + rolename) };

            var projectMember = new Member(user, user, project, roles);
            _documentSession.Store(projectMember);

            Members.Add(projectMember);
        }

        private void AddTeamMember(string userid, string teamId, string rolename)
        {
            var user = Users.Single(x => x.Id == userid);
            var team = Teams.Single(x => x.Id == teamId);
            var roles = new List<Role>() { Roles.Single(x => x.Id == "roles/" + rolename) };

            var teamMember = new Member(user, user, team, roles);

            _documentSession.Store(teamMember);

            Members.Add(teamMember);
        }

        private void AddBowerbirdAppMember(string userid, string rolename)
        {
            var user = Users.Single(x => x.Id == userid);

            var roles = new List<Role>() { Roles.Single(x => x.Id == "roles/" + rolename) };

            var appMember = new Member(user, user, TheAppRoot, roles);

            _documentSession.Store(appMember);

            Members.Add(appMember);
        }

        private void AddObservation(string userId, string projectId)
        {
            //var user = Users.Single(x => x.Id == userId);
            //var project = Projects.Single(x => x.Id == projectId);

            //var observation = new Observation(
            //    user,
            //    "Title goes here",
            //    DateTime.Now,
            //    DateTime.Now,
            //    "23.232323",
            //    "41.3432423",
            //    "1 Main St Melbourne",
            //    true,
            //    "categoryX",
            //    );

            //observation.AddGroup(project, user, DateTime.Now);

            //_documentSession.Store(observation);

            //Observations.Add(observation);
            //throw new NotImplementedException();
        }

        #endregion      
      
    }
}
