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
using Bowerbird.Core.Config;
using System.Threading;
using Bowerbird.Core.Indexes;
using Raven.Client.Linq;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.Services;
using Bowerbird.Core.Factories;

namespace Bowerbird.Core.Config
{
    public class SetupTestData
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly ISystemStateManager _systemStateManager;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IConfigService _configService;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public SetupTestData(
            IDocumentSession documentSession,
            ISystemStateManager systemStateManager,
            ICommandProcessor commandProcessor,
            IConfigService configService,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(systemStateManager, "systemStateManager");
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _systemStateManager = systemStateManager;
            _commandProcessor = commandProcessor;
            _configService = configService;
            _avatarFactory = avatarFactory;
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

        public void Execute()
        {
            try
            {
                // Disable email service to avoid emails going out to fake users/teams/orgs
                _systemStateManager.SwitchServices(enableEmails: false);

                Organisations = new List<Organisation>();
                Teams = new List<Team>();
                Projects = new List<Project>();
                Members = new List<Member>();
                Observations = new List<Observation>();
                Posts = new List<Post>();
                GroupAssociations = new List<GroupAssociation>();
                Roles = _documentSession.Query<Role>().ToList();
                Users = _documentSession.Query<User>().ToList();

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

                // Members
                AddProjectMember(Users[0].Id, Projects[0].Id, "projectadministrator", "projectmember");
                AddProjectMember(Users[0].Id, Projects[1].Id, "projectadministrator", "projectmember");
                AddProjectMember(Users[1].Id, Projects[0].Id, "projectadministrator", "projectmember");
                AddProjectMember(Users[1].Id, Projects[1].Id, "projectadministrator", "projectmember");
                AddProjectMember(Users[2].Id, Projects[0].Id, "projectadministrator", "projectmember");
                AddProjectMember(Users[2].Id, Projects[1].Id, "projectadministrator", "projectmember");

                AddTeamMember(Users[0].Id, Teams[0].Id, "teamadministrator", "teammember");
                AddTeamMember(Users[0].Id, Teams[1].Id, "teamadministrator", "teammember");
                AddTeamMember(Users[1].Id, Teams[0].Id, "teamadministrator", "teammember");
                AddTeamMember(Users[1].Id, Teams[1].Id, "teamadministrator", "teammember");
                AddTeamMember(Users[2].Id, Teams[0].Id, "teamadministrator", "teammember");
                AddTeamMember(Users[2].Id, Teams[1].Id, "teamadministrator", "teammember");

                AddOrganisationMember(Users[0].Id, Organisations[0].Id, "organisationadministrator", "organisationmember");
                AddOrganisationMember(Users[0].Id, Organisations[1].Id, "organisationadministrator", "organisationmember");
                AddOrganisationMember(Users[1].Id, Organisations[0].Id, "organisationadministrator", "organisationmember");
                AddOrganisationMember(Users[1].Id, Organisations[1].Id, "organisationadministrator", "organisationmember");
                AddOrganisationMember(Users[2].Id, Organisations[0].Id, "organisationadministrator", "organisationmember");
                AddOrganisationMember(Users[2].Id, Organisations[1].Id, "organisationadministrator", "organisationmember");

                // Save changes so that we have access to indexes for observation creation
                _documentSession.SaveChanges();
                
                // Observations
                AddObservations();

                // Save all system data now
                _documentSession.SaveChanges();

                // Wait for all stale indexes to complete.
                while (_documentSession.Advanced.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
                {
                    Thread.Sleep(1000);
                }

                // Enable emails
                _systemStateManager.SwitchServices(enableEmails: true);
            }
            catch (Exception exception)
            {
                throw new Exception("Could not setup test data", exception);
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
            var organisation = new Organisation(Users.Single(x => x.Id == userid), name, description, website, _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Organisation), DateTime.Now, TheAppRoot);
            _documentSession.Store(organisation);

            var groupAssociation = new GroupAssociation(TheAppRoot, organisation, Users.Single(x => x.Id == userid), DateTime.Now);
            GroupAssociations.Add(groupAssociation);
            _documentSession.Store(groupAssociation);

            //organisation.SetAncestry(TheAppRoot);
            _documentSession.Store(organisation);

            Organisations.Add(organisation);
        }

        private void AddTeam(string name, string description, string website, string userid, string organisationId = null)
        {
            var parentGroup = Organisations.Single(x => x.Id == organisationId);
            
            var team = new Team(Users.Single(x => x.Id == userid), name, description, website, _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Team), DateTime.Now, parentGroup);
            _documentSession.Store(team);

            var groupAssociation = new GroupAssociation(parentGroup, team, Users.Single(x => x.Id == userid), DateTime.Now);
            GroupAssociations.Add(groupAssociation);
            _documentSession.Store(groupAssociation);

            //team.SetAncestry(parentGroup);
            _documentSession.Store(team);

            parentGroup.AddDescendant(team);
            _documentSession.Store(parentGroup);

            Teams.Add(team);
        }

        private void AddProject(string name, string description, string website, string userid, string teamId = null)
        {
            var parentGroup = Teams.Single(x => x.Id == teamId);

            var project = new Project(Users.Single(x => x.Id == userid), name, description, website, _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Project), DateTime.Now, parentGroup);
            _documentSession.Store(project);


            var groupAssociation = new GroupAssociation(parentGroup, project, Users.Single(x => x.Id == userid), DateTime.Now);
            GroupAssociations.Add(groupAssociation);
            _documentSession.Store(groupAssociation);

            project.SetAncestry(parentGroup);
            _documentSession.Store(project);

            parentGroup.AddDescendant(project);
            _documentSession.Store(parentGroup);
             
            // Update parent of parent (organisation) with descendant
            var parentGroupAssociation = GroupAssociations.Single(x => x.ChildGroup.Id == parentGroup.Id);
            var organisation = Organisations.Single(x => x.Id == parentGroupAssociation.ParentGroup.Id);
            organisation.AddDescendant(project);
            _documentSession.Store(organisation);

            Projects.Add(project);
        }

        private void AddProjectMember(string userid, string projectId, params string[] roleIds)
        {
            var user = Users.Single(x => x.Id == userid);
            var project = Projects.Single(x => x.Id == projectId);
            var roles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));

            var projectMember = new Member(user, user, project, roles);
            _documentSession.Store(projectMember);

            Members.Add(projectMember);
        }

        private void AddTeamMember(string userid, string teamId, params string[] roleIds)
        {
            var user = Users.Single(x => x.Id == userid);
            var team = Teams.Single(x => x.Id == teamId);
            var roles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));

            var teamMember = new Member(user, user, team, roles);

            _documentSession.Store(teamMember);

            Members.Add(teamMember);
        }

        private void AddOrganisationMember(string userid, string organisationId, params string[] roleIds)
        {
            var user = Users.Single(x => x.Id == userid);
            var organisation = Organisations.Single(x => x.Id == organisationId);
            var roles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));

            var organisationMember = new Member(user, user, organisation, roles);

            _documentSession.Store(organisationMember);

            Members.Add(organisationMember);
        }

        private void AddBowerbirdAppMember(string userid, string rolename)
        {
            var user = Users.Single(x => x.Id == userid);

            var roles = new List<Role>() { Roles.Single(x => x.Id == "roles/" + rolename) };

            var appMember = new Member(user, user, TheAppRoot, roles);

            _documentSession.Store(appMember);

            Members.Add(appMember);
        }

        private int _observationCount = 0;

        private void AddObservations()
        {
            var userIds = Users.Select(x => x.Id);

            var userProjects = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "userproject" && x.UserIds.Any(y => y.In(userIds)))
                .ToList()
                .Select(x => x.UserProject);

            var projects = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "project" && x.UserIds.Any(y => y.In(userIds)))
                .ToList()
                .Select(x => x.Project);

            AddObservation(Users[1].Id, 0, userProjects.Single(x => x.User.Id == Users[1].Id), projects);
            AddObservation(Users[2].Id, 3, userProjects.Single(x => x.User.Id == Users[2].Id), projects.Where(x => x.Id == Projects[1].Id));            
            AddObservation(Users[0].Id, 3, userProjects.Single(x => x.User.Id == Users[0].Id), projects.Where(x => x.Id == Projects[1].Id));
            AddObservation(Users[0].Id, 4, userProjects.Single(x => x.User.Id == Users[0].Id), projects);
            //AddObservation(Users[2].Id, 5, userProjects.Single(x => x.User.Id == Users[2].Id), projects);
            //AddObservation(Users[1].Id, 5, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[0].Id));
            //AddObservation(Users[0].Id, 0, userProjects.Single(x => x.User.Id == Users[0].Id), projects.Where(x => x.Id == Projects[0].Id));
            //AddObservation(Users[1].Id, 6, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[1].Id));
            //AddObservation(Users[2].Id, 4, userProjects.Single(x => x.User.Id == Users[2].Id), projects.Where(x => x.Id == Projects[0].Id));
            //AddObservation(Users[0].Id, 2, userProjects.Single(x => x.User.Id == Users[0].Id), projects);
            //AddObservation(Users[1].Id, 2, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[1].Id));
            //AddObservation(Users[0].Id, 1, userProjects.Single(x => x.User.Id == Users[0].Id), projects);
            //AddObservation(Users[1].Id, 1, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[0].Id));
            //AddObservation(Users[2].Id, 3, userProjects.Single(x => x.User.Id == Users[2].Id), projects);            
        }

        private void AddObservation(string userId, int imageId, UserProject userProject, IEnumerable<Project> projects)
        {
            var user = Users.Single(x => x.Id == userId);

            var path = string.Format(@"{0}\media\testdata\{1}.jpg", _configService.GetEnvironmentRootPath(), imageId.ToString());

            MediaResource mediaResource = null;

            using(var stream = System.IO.File.OpenRead(path))
            {
                var mediaResourceCreateCommand = new MediaResourceCreateCommand()
                {
                    OriginalFileName = "test.jpg",
                    UploadedOn = DateTime.Now,
                    Usage = "observation",
                    UserId = userId,
                    Stream = stream
                };

                _commandProcessor.Process<MediaResourceCreateCommand, MediaResource>(mediaResourceCreateCommand, x => { mediaResource = x; });

                stream.Close();
            }

            var observation = new Observation(
                user,
                string.Format("Observation {0}", _observationCount++),
                DateTime.Now,
                DateTime.Now,
                "23.232323",
                "41.3432423",
                "1 Main St Melbourne",
                true,
                false,
                "Mammals",
                userProject,
                projects,
                new[] { new Tuple<MediaResource, string, string>(mediaResource, "test", "test") });

            _documentSession.Store(observation);

            Observations.Add(observation);
        }

        #endregion      
    }
}