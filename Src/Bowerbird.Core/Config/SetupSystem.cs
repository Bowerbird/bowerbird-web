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
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
//using NLog;
using Raven.Client;
using System.Threading;
using System.IO;
using Bowerbird.Core.DomainModelFactories;

namespace Bowerbird.Core.Config
{
    public class SetupSystem
    {
        #region Members

        //private Logger _logger = LogManager.GetLogger("SetupSystem");
        private const int _testImportLimit = 300; // In test mode, the max number of species to import per kingdom

#if DEBUG
        private bool _testImport = false;
#else
        private bool _testImport = false;
#endif

        private readonly ISystemStateManager _systemStateManager;
        private readonly IConfigSettings _configSettings;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IMessageBus _messageBus;
        private readonly IDocumentStore _documentStore;
        private const string doubleSpace = "  ";
        private const string singleSpace = " ";

        #endregion

        #region Constructors

        public SetupSystem(
            ISystemStateManager systemStateManager,
            IConfigSettings configService, 
            IMediaResourceFactory mediaResourceFactory,
            IMessageBus messageBus,
            IDocumentStore documentStore)
        {
            Check.RequireNotNull(systemStateManager, "systemStateManager");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(documentStore, "documentStore");

            _systemStateManager = systemStateManager; 
            _configSettings = configService;
            _mediaResourceFactory = mediaResourceFactory;
            _messageBus = messageBus;
            _documentStore = documentStore;
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

                // Create the AppRoot to be used before the actual app root is created
                AddAppRoot();

                // Wait for indexing to finish so that we have access to AppRoot doc
                WaitForIndexingToFinish();

                // Add permissions
                AddPermissions();

                // Add roles
                AddRoles();

                // Add system admins
                AddAdminUsers();

                // Wait for indexing to finish so that we have can turn on all services
                WaitForIndexingToFinish();

                // Enable all services
                _systemStateManager.SwitchServicesOn();

                // Add species data
                AddAllSpecies();
            }
            catch (Exception exception)
            {
                //_logger.ErrorException("Could not setup system", exception);

                throw;
            }
        }

        private void WaitForIndexingToFinish()
        {
            while (_documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
            {
                Thread.Sleep(1500);
            }
        }

        private void AddAppRoot()
        {
            using (var documentSession = CreateSession())
            {
                var categories = new Dictionary<string, string>
                    {
                        {"Amphibians", "Animalia: Chordata: Amphibia"},
                        {"Birds", "Animalia: Chordata: Aves"},
                        {"Fishes", "Animalia: Chordata"},
                        {"Fungi & Lichens", "Fungi"},
                        {"Invertebrates", "Animalia"},
                        {"Mammals", "Animalia: Chordata: Mammalia"},
                        {"Others", ""},
                        {"Plants", "Plantae"},
                        {"Reptiles", "Animalia: Chordata: Reptilia"}
                    };

                // Create the AppRoot without user
                TheAppRoot = new AppRoot(DateTime.UtcNow, categories);
                documentSession.Store(TheAppRoot);

                // Save the approot to be available for all subsequent setup
                documentSession.SaveChanges();
            }
        }

        private void AddPermissions()
        {
            using (var documentSession = CreateSession())
            {
                AddPermission(PermissionNames.CreateOrganisation, "Create Organisations", "Ability to create organisations", documentSession);
                AddPermission(PermissionNames.UpdateOrganisation, "Update Organisations", "Ability to update organisations", documentSession);
                AddPermission(PermissionNames.DeleteOrganisation, "Delete Organisations", "Ability to delete organisations", documentSession);
                AddPermission(PermissionNames.CreateProject, "Create Projects", "Ability to create projects", documentSession);
                AddPermission(PermissionNames.UpdateProject, "Update Projects", "Ability to update projects", documentSession);
                AddPermission(PermissionNames.DeleteProject, "Delete Projects", "Ability to delete projects", documentSession);
                //AddPermission(PermissionNames.CreateWatchlist, "Create Watchlists", "Ability to create watchlists", documentSession);
                //AddPermission(PermissionNames.UpdateWatchlist, "Update Watchlists", "Ability to update watchlists", documentSession);
                //AddPermission(PermissionNames.DeleteWatchlist, "Delete Watchlists", "Ability to delete watchlists", documentSession);
                AddPermission(PermissionNames.CreateObservation, "Create Observations", "Ability to create observations", documentSession);
                AddPermission(PermissionNames.UpdateObservation, "Update Observations", "Ability to update observations", documentSession);
                AddPermission(PermissionNames.DeleteObservation, "Delete Observations", "Ability to delete observations", documentSession);
                AddPermission(PermissionNames.CreatePost, "Create Posts", "Ability to create posts", documentSession);
                AddPermission(PermissionNames.UpdatePost, "Update Posts", "Ability to update posts", documentSession);
                AddPermission(PermissionNames.DeletePost, "Delete Posts", "Ability to delete posts", documentSession);
                AddPermission(PermissionNames.CreateIdentification, "Create Identifications", "Ability to create identifications", documentSession);
                AddPermission(PermissionNames.UpdateIdentification, "Update Identifications", "Ability to update identifications", documentSession);
                AddPermission(PermissionNames.DeleteIdentification, "Delete Identifications", "Ability to delete identifications", documentSession);
                AddPermission(PermissionNames.CreateSightingNote, "Create Sighting Notes", "Ability to create sighting notes", documentSession);
                AddPermission(PermissionNames.UpdateSightingNote, "Update Sighting Notes", "Ability to update sighting notes", documentSession);
                AddPermission(PermissionNames.DeleteSightingNote, "Delete Sighting Notes", "Ability to delete sighting notes", documentSession);
                AddPermission(PermissionNames.CreateSpecies, "Create Species", "Ability to create species", documentSession);
                AddPermission(PermissionNames.UpdateSpecies, "Update Species", "Ability to update species", documentSession);
                AddPermission(PermissionNames.DeleteSpecies, "Delete Species", "Ability to delete species", documentSession);
                //AddPermission(PermissionNames.CreateReferenceSpecies, "Create Reference Species", "Ability to create reference species", documentSession);
                //AddPermission(PermissionNames.UpdateReferenceSpecies, "Update Reference Species", "Ability to update reference species", documentSession);
                //AddPermission(PermissionNames.DeleteReferenceSpecies, "Delete Reference Species", "Ability to delete reference species", documentSession);
                AddPermission(PermissionNames.Chat, "Chat", "Chat with other users", documentSession);

                documentSession.SaveChanges();
            }
        }

        private void AddPermission(string id, string name, string description, IDocumentSession documentSession)
        {
            var permission = new Permission(id, name, description);

            documentSession.Store(permission);

            Permissions2.Add(permission);
        }

        private void AddRoles()
        {
            using (var documentSession = CreateSession())
            {
                AddRole("globaladministrator", "Global Administrator", "Administrator of Bowerbird",
                        documentSession,
                        PermissionNames.CreateOrganisation,
                        PermissionNames.UpdateOrganisation,
                        PermissionNames.DeleteOrganisation,
                        PermissionNames.CreateProject,
                        PermissionNames.UpdateProject,
                        PermissionNames.DeleteProject,
                        PermissionNames.CreateSpecies,
                        PermissionNames.UpdateSpecies,
                        PermissionNames.DeleteSpecies);
                AddRole("globalmoderator", "Global Community Moderator", "Community moderator of Bowerbird",
                        documentSession);
                AddRole("globalmember", "Global Member", "Member of Bowerbird",
                        documentSession,
                        PermissionNames.CreateObservation,
                        PermissionNames.UpdateObservation,
                        PermissionNames.DeleteObservation,
                        PermissionNames.CreateProject,
                        PermissionNames.Chat);
                AddRole("organisationadministrator", "Organisation Administrator", "Administrator of an organisation",
                        documentSession,
                        PermissionNames.UpdateOrganisation);
                AddRole("organisationmember", "Organisation Member", "Member of an organisation",
                        documentSession,
                        PermissionNames.CreatePost,
                        PermissionNames.UpdatePost,
                        PermissionNames.DeletePost,
                        PermissionNames.Chat);
                AddRole("favouritesadministrator", "Favourites Administrator", "Administrator of a favourites group",
                        documentSession);
                AddRole("favouritesmember", "Favourites Member", "Member of a favourites group",
                        documentSession);
                AddRole("projectadministrator", "Project Administrator", "Administrator of a project",
                        documentSession,
                        PermissionNames.UpdateProject);
                AddRole("projectmember", "Project Member", "Member of a project",
                        documentSession,
                        PermissionNames.CreateObservation,
                        PermissionNames.UpdateObservation,
                        PermissionNames.DeleteObservation,
                        PermissionNames.CreatePost,
                        PermissionNames.UpdatePost,
                        PermissionNames.DeletePost,
                        PermissionNames.CreateSightingNote,
                        PermissionNames.UpdateSightingNote,
                        PermissionNames.DeleteSightingNote,
                        PermissionNames.Chat);
                AddRole("userprojectadministrator", "User Project Administrator", "Administrator of a user project",
                        documentSession,
                        PermissionNames.CreateObservation,
                        PermissionNames.UpdateObservation,
                        PermissionNames.DeleteObservation);
                AddRole("userprojectmember", "User Project Member", "Member of a user project",
                        documentSession);

                documentSession.SaveChanges();
            }
        }

        private void AddRole(string id, string name, string description, IDocumentSession documentSession, params string[] permissionIds)
        {
            var permissions = Permissions2.Where(x => permissionIds.Any(y => x.Id == "permissions/" + y));

            var role = new Role(id, name, description, permissions);

            documentSession.Store(role);

            Roles.Add(role);
        }

        private void AddAdminUsers()
        {
            using (var documentSession = CreateSession())
            {
                AddUser("password", "frank@radocaj.com", "Frank Radocaj", documentSession, "globaladministrator", "globalmember");

                AddUser("password", "hcrittenden@museum.vic.gov.au", "Hamish Crittenden", documentSession, "globaladministrator", "globalmember");

                AddUser("password", "kwalker@museum.vic.gov.au", "Ken Walker", documentSession, "globaladministrator", "globalmember");

                // Set the user now that we have one
                TheAppRoot.SetCreatedByUser(Users.First());
                documentSession.Store(TheAppRoot);

                documentSession.SaveChanges();
            }
        }

        private void AddUser(string password, string email, string name, IDocumentSession documentSession, params string[] roleIds)
        {
            var defaultAvatarImage = _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.User);
            var defaultBackgroundImage = _mediaResourceFactory.MakeDefaultBackgroundImage("user");

            var user = new User(password, email, name, defaultAvatarImage, 
                Constants.DefaultLicence, Constants.DefaultTimezone, DateTime.UtcNow);
            documentSession.Store(user);

            user.UpdateMembership(user,
                TheAppRoot,
                Roles.Where(x => roleIds.Any(y => x.Id == "roles/" + y)));
            documentSession.Store(user);

            var userProject = new UserProject(user, name, string.Empty, string.Empty, defaultAvatarImage, defaultBackgroundImage, DateTime.UtcNow, TheAppRoot);
            documentSession.Store(userProject);

            user.UpdateMembership(
                user,
                userProject,
                Roles.Where(x => x.Id == "roles/userprojectadministrator" || x.Id == "roles/userprojectmember"));
            documentSession.Store(user);

            var favourites = new Favourites(user, DateTime.UtcNow, TheAppRoot);
            documentSession.Store(favourites);

            user.UpdateMembership(
                user,
                favourites,
                Roles.Where(x => x.Id == "roles/favouritesadministrator" || x.Id == "roles/favouritesmember"));
            documentSession.Store(user);

            Users.Add(user);

            //UserCreateCommand command = new UserCreateCommand()
            //{
            //    Name = name,
            //    Email = email,
            //    Password = password,
            //    Roles = new[] { "roles/globalmember", "roles/globaladministrator" } 
            //};

            //_commandProcessor.Process<UserCreateCommand, User>(command, x => Users.Add(x));
        }

        private void AddAllSpecies()
        {
            var fileList = Directory.GetFiles(Path.Combine(_configSettings.GetEnvironmentRootPath(), _configSettings.GetSpeciesRelativePath()));

            foreach (var file in fileList.Where(x => !Path.GetFileName(x).StartsWith("UTF8-")))
            {
                AddSpeciesKingdom(file);
            }
        }

        private bool HasImportLimitBeenReached(int count)
        {
            if (_testImport)
            {
                return count > _testImportLimit;
            }

            return false;
        }

        private void AddSpeciesKingdom(string file)
        {
            var count = 0;

            var newFile = Path.Combine(_configSettings.GetEnvironmentRootPath(), _configSettings.GetSpeciesRelativePath(), "UTF8-" + Path.GetFileName(file));

            using (StreamReader reader = new StreamReader(file, Encoding.GetEncoding("iso-8859-1")))
            {
                using (StreamWriter writer = new StreamWriter(newFile, false, Encoding.UTF8))
                {
                    writer.Write(reader.ReadToEnd());
                }
            }

            using (var reader = new StreamReader(File.OpenRead(newFile)))
            {
                while (reader.Peek() > 0 && !HasImportLimitBeenReached(count))
                {
                    using (var documentSession = CreateSession())
                    {
                        // This loop batches the ravendb inserts into multiple document sessions, according to best practices for bulk importing
                        for (int i = 0; i < 2048; i++)
                        {
                            string line = reader.ReadLine();

                            if (line == null)
                            {
                                break;
                            }

                            count++;

                            if (HasImportLimitBeenReached(count))
                            {
                                break;
                            }

                            var speciesRecord = line
                                .Split(new[] { '\t' }, StringSplitOptions.None)
                                .Where(x => x.Trim().Length > 0)
                                .ToArray();

                            AddSpecies(speciesRecord, documentSession);
                        }

                        documentSession.SaveChanges();
                    } 
                }
            }

            File.Delete(newFile);
        }

        private void AddSpecies(string[] speciesRecord, IDocumentSession documentSession)
        {
            try
            {
                if (speciesRecord.Count() < 8)
                {
                    /*
                    _logger.Log(LogLevel.Error, 
                                "Record could not be imported, less than 8 columns encountered: {0}",
                                string.Join("|", speciesRecord));
                     */
                    return;
                }

                if (speciesRecord.Count() < 8)
                {
                    /*
                    _logger.Log(LogLevel.Error,
                                "Record could not be imported, less than 8 columns encountered: {0}",
                                string.Join("|", speciesRecord));
                     */
                    return;
                }

                string categoryName = CleanCategoryString(speciesRecord[0]);

                IEnumerable<string> commonGroupNames = new string[] {};
                IEnumerable<string> commonNames = new string[] {};

                var kingdomName = string.Empty;
                var phylumName = string.Empty;
                var className = string.Empty;
                var orderName = string.Empty;
                var familyName = string.Empty;
                var genusName = string.Empty;
                var speciesName = string.Empty;
                var subSpeciesName = string.Empty;
                var synonymName = string.Empty;

                if (categoryName.ToLower() == "minerals")
                {
                    kingdomName = "Minerals";
                    speciesName = speciesRecord[7];
                }
                else
                {
                    #region Common Names

                    commonGroupNames = speciesRecord[1]
                        .Replace(@"""", " ")
                        .Replace(" or ", ", ")
                        .Replace(" and ", ", ")
                        .Replace(" & ", ", ")
                        .Replace(" - ", ", ")
                        .Replace(";", ", ")
                        .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => CleanCommonGroupNameString(x))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();

                    commonNames = speciesRecord[2]
                        .Replace(@"""", " ")
                        .Replace(" or ", ", ")
                        .Replace(" and ", ", ")
                        .Replace(" & ", ", ")
                        .Replace(" - ", ", ")
                        .Replace(";", ", ")
                        .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => CleanCommonNameString(x))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();

                    #endregion

                    #region Scientific Names

                    var taxonomyData = speciesRecord[3]
                        .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => RemoveExtraSpaces(x))
                        .ToArray();

                    if (taxonomyData[0].ToLower() == "basidiomycota" || taxonomyData[0].ToLower() == "ascomycota")
                    {
                        // We add the Fungi Kingdom manually due to source data not explicitly having it in many cases
                        kingdomName = "Fungi";
                        phylumName = CleanPhylumString(taxonomyData[0]);
                        className = taxonomyData.Length > 1 ? CleanClassString(taxonomyData[1]) : string.Empty;
                    }
                    else
                    {
                        kingdomName = CleanKingdomString(taxonomyData[0]);
                        phylumName = taxonomyData.Length > 1 ? CleanPhylumString(taxonomyData[1]) : string.Empty;
                        className = taxonomyData.Length > 2 ? CleanClassString(taxonomyData[2]) : string.Empty;
                    }

                    orderName = CleanOrderString(speciesRecord[4]);
                    familyName = CleanFamilyString(speciesRecord[5]);
                    genusName = CleanGenusString(speciesRecord[6]);

                    if (speciesRecord[7].ToLower().Contains("var.") || speciesRecord[7].ToLower().Contains("subsp."))
                        // We have potentially found a species that might have a subspecies defined in the same column
                    {
                        var speciesData = speciesRecord[7].Split(new[] {"var.", "subsp."}, StringSplitOptions.RemoveEmptyEntries);

                        speciesName = CleanSpeciesString(speciesData[0]);

                        // if we have anything left over, it is a subspecies
                        subSpeciesName = CleanSubSpeciesString(string.Join(" ", speciesData.Skip(1).Take(100)));
                    }
                    else
                    {
                        speciesName = CleanSpeciesString(speciesRecord[7]);
                    }

                    if (speciesRecord.Count() > 8)
                    {
                        var synonymData = speciesRecord
                            .Skip(8)
                            .Take(10)
                            .Select(x => RemoveExtraSpaces(x))
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Where(x => x.ToLower() != "syn");

                        synonymName = string.Join(" ", synonymData);
                    }

                    #endregion
                }

                if (!string.IsNullOrWhiteSpace(speciesName) && speciesName.ToLower() != "sp.") // Don't save records that have no species name and that are species that are only "sp." placeholders
                {
                    documentSession.Store(
                        new Species(
                            categoryName,
                            commonGroupNames,
                            commonNames,
                            kingdomName,
                            phylumName,
                            className,
                            orderName,
                            familyName,
                            genusName,
                            speciesName,
                            subSpeciesName,
                            synonymName,
                            DateTime.UtcNow,
                            Users[0])
                        );
                }
            }
            catch (Exception ex)
            {
                //_logger.ErrorException("Record could not be imported, unknown error: " + string.Join("|", speciesRecord), ex);
            }
        }

        private string CleanCategoryString(string val)
        {
            return RemoveExtraSpaces(val);
        }

        private string CleanCommonNameString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanCommonGroupNameString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanKingdomString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanPhylumString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanClassString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanOrderString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanFamilyString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanGenusString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemovePunctuationAndQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanSpeciesString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemoveQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string CleanSubSpeciesString(string val)
        {
            var newVal = RemoveKeywords(val);
            newVal = RemoveQuotes(newVal);
            return RemoveExtraSpaces(newVal);
        }

        private string RemoveKeywords(string val)
        {
            return val
                .Replace("Unknown", singleSpace)
                .Replace("unknown", singleSpace)
                .Replace("Genus Unknown", singleSpace)
                .Replace("genus unknown", singleSpace)
                .Replace("Genus unknown", singleSpace)
                .Replace("Not Applicable", singleSpace)
                .Replace("Not applicable", singleSpace)
                .Replace("not applicable", singleSpace)
                .Replace("Not Assigned", singleSpace)
                .Replace("Not assigned", singleSpace)
                .Replace("not assigned", singleSpace)
                .Replace("None Assigned", singleSpace)
                .Replace("None assigned", singleSpace)
                .Replace("none assigned", singleSpace)
                .Replace("Unplaced", singleSpace)
                .Replace("unplaced", singleSpace)
                .Replace("Unranked", singleSpace)
                .Replace("unranked", singleSpace)
                .Replace("Incertae Sedis", singleSpace)
                .Replace("Incertae sedis", singleSpace)
                .Replace("incertae sedis", singleSpace);
        }

        private string RemoveQuotes(string val)
        {
            var newVal = val
                .Replace(@"""", singleSpace)
                .Trim();

            if (newVal.StartsWith("'") && newVal.EndsWith("'"))
            {
                newVal = newVal.Remove(0, 1);
                newVal = newVal.Remove(newVal.Length - 1, 1);
            }

            return newVal;
        }

        private string RemovePunctuationAndQuotes(string val)
        {
            var newVal = val
                .Replace("[", singleSpace)
                .Replace("]", singleSpace)
                .Replace("(", singleSpace)
                .Replace(")", singleSpace)
                .Replace(".", singleSpace)
                .Replace(",", singleSpace)
                .Replace('\t', ' ')
                .Trim();

            return RemoveQuotes(newVal);
        }

        private string RemoveExtraSpaces(string val)
        {
            var newVal = val;

            while (newVal.Contains(doubleSpace))
            {
                newVal = newVal.Replace(doubleSpace, singleSpace);
            }

            return string.IsNullOrWhiteSpace(newVal) ? string.Empty : newVal.Trim();
        }

        private IDocumentSession CreateSession()
        {
            if (!string.IsNullOrWhiteSpace(_configSettings.GetDatabaseName()))
            {
                return _documentStore.OpenSession(_configSettings.GetDatabaseName());
            }
            else
            {
                return _documentStore.OpenSession();
            }
        }

        #endregion      
      
    }
}
