/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Diagnostics;
using System.IO;
using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Bowerbird.Test.Utils
{
    public class DocumentStoreHelper
    {
        private static StreamWriter _sw; // Handles strings sent to CMD.exe
        private static StreamReader _sr; // Reads text back from CMD.exe

        private static IDocumentStore RamDocumentStore(bool injectData = true)
        {
            var documentStore = new DocumentStore { Url = "http://localhost:8080/" };

            documentStore.Conventions.FindIdentityProperty = prop => prop.Name == "Id";

            documentStore.Initialize();

            if (injectData)
            {
                using (var documentSession = documentStore.OpenSession())
                {
                    // if we have no roles, system is not configured, so run system setup
                    var roles = documentSession.Query<Role>().ToList();
                    if (roles.Count == 0)
                    {
                        var systemStateManager = new SystemStateManager(documentSession);

                        var setupSystemDataCommandHander = new SetupSystemDataCommandHandler(
                            documentStore,
                            documentSession,
                            systemStateManager
                            );

                        setupSystemDataCommandHander.Handle(new SetupSystemDataCommand());
                        documentSession.SaveChanges();
                    }
                }
            }

            IndexCreation.CreateIndexes(typeof(All_Groups).Assembly, documentStore);

            return documentStore;
        }

        public static IDocumentStore StartRaven(bool injectData = true)
        {
            var dir = new Process
            {
                StartInfo =
                    {
                        FileName = "CMD.EXE",
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        Arguments = @"/K C:\Projects\bowerbird.web\Src\packages\RavenDB.1.0.665-Unstable\server\Raven.Server.exe -ram"
                    }
            };

            dir.Start();
            _sw = dir.StandardInput;
            _sr = dir.StandardOutput;
            _sw.AutoFlush = true;

            return RamDocumentStore(injectData);
        }

        public static void KillRaven()
        {
            _sw.WriteLine("q");
            _sw.Close();
            _sr.Close();
        }
    }
}