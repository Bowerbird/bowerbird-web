/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using System.IO;
using System.Linq;
using System.Threading;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.Services;

namespace Bowerbird.Core.CommandHandlers
{
    public class SetupSpeciesDataCommandHandler : ICommandHandler<SetupSpeciesDataCommand>
    {
        #region Fields

        private readonly IDocumentStore _documentStore;
        private readonly IDocumentSession _documentSession;
        private readonly ISystemStateManager _systemStateManager;
        private readonly IConfigService _configService;
        private readonly string[] _speciesFileHeaderColumns = {
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

        public SetupSpeciesDataCommandHandler(
            IDocumentStore documentStore,
            IDocumentSession documentSession,
            ISystemStateManager systemStateManager,
            IConfigService configService)
        {
            Check.RequireNotNull(documentStore, "documentStore");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(systemStateManager, "systemStateManager");
            Check.RequireNotNull(configService, "configService");

            _documentStore = documentStore;
            _documentSession = documentSession;
            _systemStateManager = systemStateManager;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(SetupSpeciesDataCommand command)
        {
            Check.RequireNotNull(command, "command");

            try
            {
                _systemStateManager.DisableEmailService();

                //var user = _documentSession.Load<User>(command.UserId);
                var createdOn = DateTime.UtcNow;

                var speciesFromFiles = LoadSpeciesFilesFromFolder(Path.Combine(_configService.GetEnvironmentRootPath(), _configService.GetSpeciesRelativePath()));

                foreach (var species in speciesFromFiles)
                {
                    _documentSession.Store(
                        new Species(
                            species[0],
                            species[1],
                            species[2].Split(',').ToArray(),
                            species[3],
                            species[4],
                            species[5],
                            species[6],
                            species[7],
                            species[8],
                            false,
                            createdOn
                            )
                        );
                }

                _documentSession.SaveChanges();

                while (_documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
                {
                    Thread.Sleep(10);
                }

                _systemStateManager.EnableEmailService();

            }
            catch (Exception exception)
            {
                throw new Exception("Could not setup species data.", exception);
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
                    var fileHeaderColumns = reader.ReadLine().Split(new[] {'\t'}, StringSplitOptions.None).Take(_speciesFileHeaderColumns.Length);
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

                    while (reader.Peek() > 0)
                    {
                        var fieldValues = reader
                            .ReadLine()
                            .Split(new[] { '\t' }, StringSplitOptions.None)
                            .Take(_speciesFileHeaderColumns.Length);

                        species.Add(fieldValues.Select(x => x.Trim()).ToList());
                    }
                }
            }

            return species;
        }

        #endregion
    }
}