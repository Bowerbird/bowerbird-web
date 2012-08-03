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
using NLog;
using Raven.Client;
using System.Threading;
using System.IO;
using Bowerbird.Core.Factories;

namespace Bowerbird.Core.Config
{
    public class SetupSystem
    {
        #region Members

        private Logger _logger = LogManager.GetLogger("SetupSystem");

        private readonly IDocumentSession _documentSession;
        private readonly ISystemStateManager _systemStateManager;
        private readonly IConfigSettings _configSettings;
        private readonly IAvatarFactory _avatarFactory;
        private readonly ICommandProcessor _commandProcessor;

        private readonly string[] _speciesFileHeaderColumns = {
                                                                  "Category", 
                                                                  "Kingdom", 
                                                                  "Group Name", 
                                                                  "Species Common Names", 
                                                                  "Taxonomy", 
                                                                  "Order", 
                                                                  "Family", 
                                                                  "Genus", 
                                                                  "Species",
                                                                  "Synonym"
                                                              };

        #endregion

        #region Constructors

        public SetupSystem(
            IDocumentSession documentSession,
            ISystemStateManager systemStateManager,
            IConfigSettings configService, 
            IAvatarFactory avatarFactory,
            ICommandProcessor commandProcessor)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(systemStateManager, "systemStateManager");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(avatarFactory, "avatarFactory");
            Check.RequireNotNull(commandProcessor, "commandProcessor");

            _documentSession = documentSession;
            _systemStateManager = systemStateManager;
            _configSettings = configService;
            _avatarFactory = avatarFactory;
            _commandProcessor = commandProcessor;
        }

        #endregion

        #region Properties

        private AppRoot TheAppRoot { get; set; }

        private List<Permission> Permissions2 { get; set; }
        
        private List<Role> Roles { get; set; }

        private List<User> Users { get; set; }

        #endregion

        #region Methods

        public void Execute()
        {
            try
            {
                Permissions2 = new List<Permission>();
                Roles = new List<Role>();
                Users = new List<User>();

                // Create the temporary AppRoot to be used before the actual app root is created
                AddAppRoot();

                // Save the approot to be available for all subsequent setup
                _documentSession.SaveChanges();

                // Add permissions first
                AddPermissions();

                // Add roles, using permissions
                AddRoles();

                // Add species data
                AddSpecies();

                _documentSession.SaveChanges();

                // Add system admins
                AddAdminUsers();

                // Set the user now that we have one
                SetAppRootUser(Users[0].Id);

                // Save all system data now
                _documentSession.SaveChanges();

                // Wait for all stale indexes to complete.
                WaitForIndexingToFinish();

                // Enable all services
                _systemStateManager.SwitchServicesOn();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Could not setup system", exception);

                throw exception;
            }
        }

        private void WaitForIndexingToFinish()
        {
            while (_documentSession.Advanced.DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
            {
                Thread.Sleep(1500);
            }
        }

        private void AddAppRoot()
        {
            var categories = new[] 
            {
                "Amphibians", 
                "Birds", 
                "Fishes", 
                "Fungi", 
                "Invertebrates", 
                "Mammals", 
                "Minerals",
                "Others",
                "Plants", 
                "Reptiles"
            };

            // Create the TempAppRoot to be used before the actual app root is created
            // Once the real temp app root is created, this one is no longer used
            TheAppRoot = new AppRoot(DateTime.UtcNow, categories);
            _documentSession.Store(TheAppRoot);
        }

        private void SetAppRootUser(string userId)
        {
            TheAppRoot.SetCreatedByUser(Users.Single(x => x.Id == userId));
            _documentSession.Store(TheAppRoot);
        }

        private void AddPermissions()
        {
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
            AddPermission(PermissionNames.CreateSpecies, "Create Species", "Ability to create species");
            AddPermission(PermissionNames.UpdateSpecies, "Update Species", "Ability to update species");
            AddPermission(PermissionNames.DeleteSpecies, "Delete Species", "Ability to delete species");
            AddPermission(PermissionNames.CreateReferenceSpecies, "Create Reference Species", "Ability to create reference species");
            AddPermission(PermissionNames.UpdateReferenceSpecies, "Update Reference Species", "Ability to update reference species");
            AddPermission(PermissionNames.DeleteReferenceSpecies, "Delete Reference Species", "Ability to delete reference species");
            AddPermission(PermissionNames.Chat, "Chat", "Chat with othet users");
        }

        private void AddPermission(string id, string name, string description)
        {
            var permission = new Permission(id, name, description);

            _documentSession.Store(permission);

            Permissions2.Add(permission);
        }

        private void AddRoles()
        {
            AddRole("globaladministrator", "Global Administrator", "Administrator of Bowerbird",
                PermissionNames.CreateOrganisation,
                PermissionNames.UpdateOrganisation,
                PermissionNames.DeleteOrganisation,
                PermissionNames.CreateTeam,
                PermissionNames.UpdateTeam,
                PermissionNames.DeleteTeam,
                PermissionNames.CreateProject,
                PermissionNames.UpdateProject,
                PermissionNames.DeleteProject,
                PermissionNames.CreateSpecies,
                PermissionNames.UpdateSpecies,
                PermissionNames.DeleteSpecies);
            AddRole("globalmoderator", "Global Community Moderator", "Comunity moderator of Bowerbird",
                PermissionNames.CreateReferenceSpecies,
                PermissionNames.UpdateReferenceSpecies,
                PermissionNames.DeleteReferenceSpecies);
            AddRole("globalmember", "Global Member", "Member of Bowerbird",
                PermissionNames.CreateObservation,
                PermissionNames.UpdateObservation,
                PermissionNames.DeleteObservation,
                PermissionNames.CreateProject,
                PermissionNames.UpdateProject,
                PermissionNames.DeleteProject,
                PermissionNames.Chat);
            AddRole("organisationadministrator", "Organisation Administrator", "Administrator of an organisation",
                PermissionNames.UpdateOrganisation,
                PermissionNames.CreateTeam,
                PermissionNames.UpdateTeam,
                PermissionNames.DeleteTeam);
            AddRole("organisationmember", "Organisation Member", "Member of an organisation",
                PermissionNames.CreatePost,
                PermissionNames.UpdatePost,
                PermissionNames.DeletePost,
                PermissionNames.Chat);
            AddRole("teamadministrator", "Team Administrator", "Administrator of a team",
                PermissionNames.UpdateTeam,
                PermissionNames.CreateProject,
                PermissionNames.UpdateProject,
                PermissionNames.DeleteProject);
            AddRole("teammember", "Team Member", "Member of a team",
                PermissionNames.CreatePost,
                PermissionNames.UpdatePost,
                PermissionNames.DeletePost,
                PermissionNames.Chat);
            AddRole("projectadministrator", "Project Administrator", "Administrator of a project",
                PermissionNames.UpdateProject);
            AddRole("projectmember", "Project Member", "Member of a project",
                PermissionNames.CreateObservation,
                PermissionNames.UpdateObservation,
                PermissionNames.DeleteObservation,
                PermissionNames.CreatePost,
                PermissionNames.UpdatePost,
                PermissionNames.DeletePost,
                PermissionNames.Chat);
            AddRole("userprojectadministrator", "User Project Administrator", "Administrator of a user project",
                PermissionNames.UpdateProject,
                PermissionNames.CreatePost,
                PermissionNames.UpdatePost,
                PermissionNames.DeletePost);
            AddRole("userprojectmember", "User Project Member", "Member of a user project",
                PermissionNames.CreateObservation,
                PermissionNames.UpdateObservation,
                PermissionNames.DeleteObservation,
                PermissionNames.CreatePost,
                PermissionNames.UpdatePost,
                PermissionNames.DeletePost);
        }

        private void AddRole(string id, string name, string description, params string[] permissionIds)
        {
            var permissions = Permissions2.Where(x => permissionIds.Any(y => x.Id == "permissions/" + y));

            var role = new Role(id, name, description, permissions);

            _documentSession.Store(role);

            Roles.Add(role);
        }

        private void AddAdminUsers()
        {
            AddUser("password", "frank@radocaj.com", "Frank", "Radocaj", "globaladministrator", "globalmember");
            _documentSession.SaveChanges();

            AddUser("password", "hcrittenden@museum.vic.gov.au", "Hamish", "Crittenden", "globaladministrator", "globalmember");
            _documentSession.SaveChanges();

            AddUser("password", "kwalker@museum.vic.gov.au", "Ken", "Walker", "globaladministrator", "globalmember");
            _documentSession.SaveChanges();
        }

        private void AddUser(string password, string email, string firstname, string lastname, params string[] roleIds)
        {
            var user = new User(password, email, firstname, lastname, _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.User), Constants.DefaultLicence);
            _documentSession.Store(user);

            user.AddMembership(user,
                TheAppRoot,
                Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y)));
            _documentSession.Store(user);

            var userProject = new UserProject(user, DateTime.UtcNow, TheAppRoot);
            _documentSession.Store(userProject);

            user.AddMembership(
                user,
                userProject,
                Roles.Where(x => x.Id == "roles/userprojectadministrator" || x.Id == "roles/userprojectmember"));
            _documentSession.Store(user);

            Users.Add(user);

            //UserCreateCommand command = new UserCreateCommand()
            //{
            //    FirstName = firstname,
            //    LastName = lastname,
            //    Email = email,
            //    Password = password,
            //    Roles = new[] { "roles/globalmember", "roles/globaladministrator" } 
            //};

            //_commandProcessor.Process<UserCreateCommand, User>(command, x => Users.Add(x));
        }

        private void AddSpecies()
        {
            var createdOn = DateTime.UtcNow;

            var speciesFromFiles = LoadSpeciesFilesFromFolder(Path.Combine(_configSettings.GetEnvironmentRootPath(), _configSettings.GetSpeciesRelativePath()));

            foreach (var species in speciesFromFiles)
            {
                _documentSession.Store(
                    new Species(
                        species[0],
                        species[1],
                        species[2],
                        species[3].Split(',').ToArray(),
                        species[4],
                        species[5],
                        species[6],
                        species[7],
                        species[8],
                        species[9],
                        false,
                        createdOn
                        )
                    );
            }
        }

        private IEnumerable<List<string>> LoadSpeciesFilesFromFolder(string folderPath)
        {
            var species = new List<List<string>>();

            var fileList = Directory.GetFiles(folderPath);

            foreach (var file in fileList)
            {
                using (var reader = new StreamReader(File.OpenRead(file)))
                {
                    var fileHeaderColumns = reader.ReadLine().Split(new[] { '\t' }, StringSplitOptions.None).Take(_speciesFileHeaderColumns.Length);
                    var counter = 0;

                    foreach (var col in fileHeaderColumns)
                    {
                        if (!_speciesFileHeaderColumns[counter].ToLower().Equals(col.ToLower()))
                        {
                            throw new ApplicationException(
                                String.Format(
                                    "The header for column number {0} is {1} but should be {2} in species upload file {3}",
                                    counter + 1,
                                    col,
                                    _speciesFileHeaderColumns[counter],
                                    file
                                    ));
                        }
                        counter++;
                    }
                    var count = 0;
                    while (reader.Peek() > 0 && count < 100) // HACK: Only load 100 species for now
                    {
                        var fieldValues = reader
                            .ReadLine()
                            .Split(new[] { '\t' }, StringSplitOptions.None)
                            .Take(_speciesFileHeaderColumns.Length);

                        species.Add(fieldValues.Select(x => x.Trim()).ToList());
                        count++;
                    }
                }
            }

            return species;
        }

        #endregion      
      
    }
}
