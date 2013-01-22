///* Bowerbird V1 

// Licensed under MIT 1.1 Public License
// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Bowerbird.Core.Commands;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.Infrastructure;
//using NLog;
//using Raven.Client;
//using System.Threading;
//using Bowerbird.Core.Indexes;
//using Raven.Client.Linq;
//using Bowerbird.Core.Factories;

//namespace Bowerbird.Core.Config
//{
//    public class SetupTestData
//    {
//        #region Members

//        private Logger _logger = LogManager.GetLogger("SetupTestData");

//        private readonly IDocumentSession _documentSession;
//        private readonly ISystemStateManager _systemStateManager;
//        private readonly IMessageBus _messageBus;
//        private readonly IConfigSettings _configSettings;
//        private readonly IMediaResourceFactory _mediaResourceFactory;

//        #endregion

//        #region Constructors

//        public SetupTestData(
//            IDocumentSession documentSession,
//            ISystemStateManager systemStateManager,
//            IMessageBus messageBus,
//            IConfigSettings configService,
//            IMediaResourceFactory mediaResourceFactory)
//        {
//            Check.RequireNotNull(documentSession, "documentSession");
//            Check.RequireNotNull(systemStateManager, "systemStateManager");
//            Check.RequireNotNull(messageBus, "messageBus");
//            Check.RequireNotNull(configService, "configService");
//            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");

//            _documentSession = documentSession;
//            _systemStateManager = systemStateManager;
//            _messageBus = messageBus;
//            _configSettings = configService;
//            _mediaResourceFactory = mediaResourceFactory;
//        }

//        #endregion

//        #region Properties

//        private AppRoot TheAppRoot { get; set; }

//        private List<Role> Roles { get; set; }

//        private List<User> Users { get; set; }

//        private List<Organisation> Organisations { get; set; }

//        private List<Team> Teams { get; set; }

//        private List<Project> Projects { get; set; }

//        private List<Observation> Observations { get; set; }

//        private List<Post> Posts { get; set; }

//        //private List<GroupAssociation> GroupAssociations { get; set; }

//        #endregion

//        #region Methods

//        public void Execute()
//        {
//            try
//            {
//                // Disable all services
//                _systemStateManager.SwitchServicesOff();

//                Organisations = new List<Organisation>();
//                Teams = new List<Team>();
//                Projects = new List<Project>();
//                Observations = new List<Observation>();
//                Posts = new List<Post>();
//                //GroupAssociations = new List<GroupAssociation>();
//                Roles = _documentSession.Query<Role>().ToList();
//                Users = _documentSession.Query<User>().ToList();

//                TheAppRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

//                // Users
//                //AddUser("password", "frank@radocaj.com", "Frank", "Radocaj", "globaladministrator", "globalmember");

//                // Organisations
//                AddOrganisation("Test Organisation", "Test organisation for Alpha release. " + GetLoremIpsum(), "www.bowerbird.org.au", Users[0].Id);
//                //AddOrganisation("Museum Victoria", "Museum Victoria", "www.museumvictoria.com", Users[0].Id);

//                // Teams
//                //AddTeam("Test Team", "Test team for Alpha release. " + GetLoremIpsum(), "www.bowerbird.org.au", Users[0].Id, Organisations[0].Id);
//                //AddTeam("Ken Walker tests Bowerbird", "Another Test team for Alpha Release", "www.bowerbird.org.au", Users[0].Id, Organisations[1].Id);

//                // Projects
//                //AddProject("Test Project", "Test project for Alpha release. " + GetLoremIpsum(), "www.bowerbird.org.au", Users[0].Id, Teams[0].Id);
//                //AddProject("Kens Bees", "Bee Project", "www.bowerbird.org.au", Users[0].Id, Teams[1].Id);

//                //// Save changes so that we have access to indexes for observation creation
//                //_documentSession.SaveChanges();

//                //// Wait for all stale indexes to complete.
//                //WaitForIndexingToFinish();
                
//                // Members
//                //AddProjectMember(Users[0].Id, Projects[0].Id, "projectadministrator", "projectmember");
//                //AddProjectMember(Users[0].Id, Projects[1].Id, "projectadministrator", "projectmember");
//                //AddProjectMember(Users[1].Id, Projects[0].Id, "projectadministrator", "projectmember");
//                //AddProjectMember(Users[1].Id, Projects[1].Id, "projectadministrator", "projectmember");
//                //AddProjectMember(Users[2].Id, Projects[0].Id, "projectadministrator", "projectmember");
//                //AddProjectMember(Users[2].Id, Projects[1].Id, "projectadministrator", "projectmember");

//                //AddTeamMember(Users[0].Id, Teams[0].Id, "teamadministrator", "teammember");
//                //AddTeamMember(Users[0].Id, Teams[1].Id, "teamadministrator", "teammember");
//                //AddTeamMember(Users[1].Id, Teams[0].Id, "teamadministrator", "teammember");
//                //AddTeamMember(Users[1].Id, Teams[1].Id, "teamadministrator", "teammember");
//                //AddTeamMember(Users[2].Id, Teams[0].Id, "teamadministrator", "teammember");
//                //AddTeamMember(Users[2].Id, Teams[1].Id, "teamadministrator", "teammember");

//                //AddOrganisationMember(Users[0].Id, Organisations[0].Id, "organisationadministrator", "organisationmember");
//                //AddOrganisationMember(Users[0].Id, Organisations[1].Id, "organisationadministrator", "organisationmember");
//                //AddOrganisationMember(Users[1].Id, Organisations[0].Id, "organisationadministrator", "organisationmember");
//                //AddOrganisationMember(Users[1].Id, Organisations[1].Id, "organisationadministrator", "organisationmember");
//                //AddOrganisationMember(Users[2].Id, Organisations[0].Id, "organisationadministrator", "organisationmember");
//                //AddOrganisationMember(Users[2].Id, Organisations[1].Id, "organisationadministrator", "organisationmember");

//                //// Save changes so that we have access to indexes for observation creation
//                //_documentSession.SaveChanges();

//                //// Wait for all stale indexes to complete.
//                //WaitForIndexingToFinish();
                
//                //// Observations
//                //AddObservations();

//                // Save all system data now
//                _documentSession.SaveChanges();

//                // Wait for all stale indexes to complete.
//                WaitForIndexingToFinish();

//                // Enable all services
//                _systemStateManager.SwitchServicesOn();
//            }
//            catch (Exception exception)
//            {
//                _logger.ErrorException("Could not setup test data", exception);

//                throw exception;
//            }
//        }

//        private void WaitForIndexingToFinish()
//        {
//            while (_documentSession.Advanced.DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
//            {
//                Thread.Sleep(1500);
//            }
//        }

//        //private void AddUser(string password, string email, string name, params string[] roleIds)
//        //{
//        //    var globalRoles = Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y));
//        //    var user = new User(password, email, name, globalRoles);
//        //    _documentSession.Store(user);
//        //    Users.Add(user);

//        //    var userProject = new UserProject(user);
//        //    _documentSession.Store(userProject);

//        //    var userProjectRoles = Roles.Where(x => x.Id == "roles/projectadministrator" || x.Id == "roles/projectmember");
//        //    var groupMember = new GroupMember(user, userProject, user, userProjectRoles);
//        //    _documentSession.Store(groupMember);
//        //}

//        private void AddOrganisation(string name, string description, string website, string userid)
//        {
//            var user = Users.Single(x => x.Id == userid);
//            var organisation = new Organisation(user, name, description, website, _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Organisation), DateTime.UtcNow, TheAppRoot);
//            _documentSession.Store(organisation);

//            //_documentSession.Store(organisation);

//            Organisations.Add(organisation);

//            // Add administrator membership to creating user
//            user
//                .AddMembership(
//                user,
//                organisation,
//                _documentSession
//                    .Query<Role>()
//                    .Where(x => x.Id.In("roles/organisationadministrator", "roles/organisationmember"))
//                    .ToList());
//            _documentSession.Store(user);
//        }

//        private void AddTeam(string name, string description, string website, string userid, string organisationId = null)
//        {
//            var user = Users.Single(x => x.Id == userid);
//            var parentGroup = Organisations.Single(x => x.Id == organisationId);

//            var team = new Team(user, name, description, website, _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Team), DateTime.UtcNow, parentGroup);
//            _documentSession.Store(team);

//            _documentSession.Store(team);

//            parentGroup.AddChildGroup(team);
//            _documentSession.Store(parentGroup);

//            Teams.Add(team);

//            // Add administrator membership to creating user
//            user
//                .AddMembership(
//                user,
//                team,
//                _documentSession
//                    .Query<Role>()
//                    .Where(x => x.Id.In("roles/teamadministrator", "roles/teammember"))
//                    .ToList());
//            _documentSession.Store(user);
//        }

//        private void AddProject(string name, string description, string website, string userid, string teamId = null)
//        {
//            var user = Users.Single(x => x.Id == userid);
//            var parentGroup = Teams.Single(x => x.Id == teamId);

//            var project = new Project(user, name, description, website, _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Project), _mediaResourceFactory.MakeDefaultBackgroundImage("project"), DateTime.UtcNow, parentGroup);
//            _documentSession.Store(project);

//            project.AddParentGroup(parentGroup);
//            _documentSession.Store(project);

//            parentGroup.AddChildGroup(project);
//            _documentSession.Store(parentGroup);
             
//            // Update parent of parent (organisation) with descendant
//            var organisation = Organisations.First(y => y.Id == parentGroup.AncestorGroups.First(x => x.GroupType == "organisation").Id);
//            organisation.AddDescendantGroup(project);
//            _documentSession.Store(organisation);

//            Projects.Add(project);

//            // Add administrator membership to creating user
//            user
//                .AddMembership(
//                user,
//                project,
//                _documentSession
//                    .Query<Role>()
//                    .Where(x => x.Id.In("roles/projectadministrator", "roles/projectmember"))
//                    .ToList());
//            _documentSession.Store(user);
//        }

//        private void AddProjectMember(string userid, string projectId, params string[] roleIds)
//        {
//            var user = Users.Single(x => x.Id == userid);

//            user.AddMembership(
//                user,
//                Projects.Single(x => x.Id == projectId),
//                Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y)));

//            _documentSession.Store(user);
//        }

//        private void AddTeamMember(string userid, string teamId, params string[] roleIds)
//        {
//            var user = Users.Single(x => x.Id == userid);

//            user.AddMembership(
//                user,
//                Teams.Single(x => x.Id == teamId),
//                Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y)));

//            _documentSession.Store(user);
//        }

//        private void AddOrganisationMember(string userid, string organisationId, params string[] roleIds)
//        {
//            var user = Users.Single(x => x.Id == userid);

//            user.AddMembership(
//                user,
//                Organisations.Single(x => x.Id == organisationId),
//                Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y)));

//            _documentSession.Store(user);
//        }

//        private void AddBowerbirdAppMember(string userid, string rolename)
//        {
//            var user = Users.Single(x => x.Id == userid);

//            user.AddMembership(
//                user,
//                TheAppRoot,
//                new[] { Roles.Single(x => x.Id == "roles/" + rolename) });

//            _documentSession.Store(user);
//        }

//        //private int _observationCount = 0;

//        private void AddObservations()
//        {
//            var userIds = Users.Select(x => x.Id);

//            var userProjects = _documentSession
//                .Query<All_Groups.Result, All_Groups>()
//                .AsProjection<All_Groups.Result>()
//                .Where(x => x.GroupType == "userproject" && x.UserIds.Any(y => y.In(userIds)))
//                .ToList()
//                .Select(x => x.UserProject);

//            var projects = _documentSession
//                .Query<All_Groups.Result, All_Groups>()
//                .AsProjection<All_Groups.Result>()
//                .Where(x => x.GroupType == "project" && x.UserIds.Any(y => y.In(userIds)))
//                .ToList()
//                .Select(x => x.Project);

//            //AddObservation(Users[1].Id, 0, userProjects.Single(x => x.User.Id == Users[1].Id), projects);
//            //AddObservation(Users[2].Id, 3, userProjects.Single(x => x.User.Id == Users[2].Id), projects.Where(x => x.Id == Projects[1].Id));            
//            //AddObservation(Users[0].Id, 3, userProjects.Single(x => x.User.Id == Users[0].Id), projects.Where(x => x.Id == Projects[1].Id));
//            //AddObservation(Users[0].Id, 4, userProjects.Single(x => x.User.Id == Users[0].Id), projects);
//            //AddObservation(Users[2].Id, 5, userProjects.Single(x => x.User.Id == Users[2].Id), projects);
//            //AddObservation(Users[1].Id, 5, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[0].Id));
//            //AddObservation(Users[0].Id, 0, userProjects.Single(x => x.User.Id == Users[0].Id), projects.Where(x => x.Id == Projects[0].Id));
//            //AddObservation(Users[1].Id, 6, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[1].Id));
//            //AddObservation(Users[2].Id, 4, userProjects.Single(x => x.User.Id == Users[2].Id), projects.Where(x => x.Id == Projects[0].Id));
//            //AddObservation(Users[0].Id, 2, userProjects.Single(x => x.User.Id == Users[0].Id), projects);
//            //AddObservation(Users[1].Id, 2, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[1].Id));
//            //AddObservation(Users[0].Id, 1, userProjects.Single(x => x.User.Id == Users[0].Id), projects);
//            //AddObservation(Users[1].Id, 1, userProjects.Single(x => x.User.Id == Users[1].Id), projects.Where(x => x.Id == Projects[0].Id));
//            //AddObservation(Users[2].Id, 3, userProjects.Single(x => x.User.Id == Users[2].Id), projects);            
//        }

//        //private void AddObservation(string userId, int imageId, UserProject userProject, IEnumerable<Project> projects)
//        //{
//        //    var user = Users.Single(x => x.Id == userId);

//        //    var path = string.Format(@"{0}\media\testdata\{1}.jpg", _configSettings.GetEnvironmentRootPath(), imageId.ToString());

//        //    using(var stream = System.IO.File.OpenRead(path))
//        //    {
//        //        var mediaResourceCreateCommand = new MediaResourceCreateCommand()
//        //        {
//        //            UploadedOn = DateTime.UtcNow,
//        //            Usage = "observation",
//        //            UserId = userId,
//        //            FileStream = stream,
//        //            FileName = "test.jpg"
//        //        };

//        //        _messageBus.Send(mediaResourceCreateCommand);

//        //        stream.Close();
//        //    }

//        //    var observation = new Observation(
//        //        user,
//        //        string.Format("Observation {0}", _observationCount++),
//        //        DateTime.UtcNow,
//        //        DateTime.UtcNow,
//        //        "23.232323",
//        //        "41.3432423",
//        //        "1 Main St Melbourne",
//        //        true,
//        //        false,
//        //        "Mammals",
//        //        userProject,
//        //        projects);

//        //    _documentSession.Store(observation);

//        //    Observations.Add(observation);
//        //}

//        private string GetLoremIpsum()
//        {
//            return "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam gravida, ipsum et congue volutpat, elit libero pretium massa, vel luctus lectus purus et lorem. In nec nisi vel ligula adipiscing bibendum a nec felis. In scelerisque massa eu velit aliquam eget ultrices mi porttitor. Integer vel ante at orci fringilla posuere vitae et ligula. Nam luctus dolor sed odio imperdiet ut vulputate neque pellentesque. Nulla mattis velit quis libero ornare ac bibendum nisl fringilla. Proin semper porta augue, at venenatis nisi pharetra eget. In posuere feugiat dui, at laoreet odio elementum mollis. Proin sed arcu enim, ac pulvinar enim. Sed id luctus tortor. Maecenas aliquam quam in nulla fermentum ornare. Praesent at ante turpis. Phasellus mattis est in sapien pretium id gravida justo mollis. Suspendisse potenti. ";
//        }

//        #endregion      
//    }
//}