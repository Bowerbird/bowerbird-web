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
using System.Configuration;

namespace Bowerbird.Test.Utils
{
    public class DocumentStoreHelper
    {
        private static StreamWriter sw; // Handles strings sent to CMD.exe
        private static StreamReader sr; // Reads text back from CMD.exe
        private static Process dir;

        private static IDocumentStore RamDocumentStore()
        {
            BootstrapperHelper.Startup();

            var documentStore = new DocumentStore { Url = "http://localhost:8080/" };

            documentStore.Conventions.FindIdentityProperty = prop => prop.Name == "Id";
            documentStore.Initialize();

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

            IndexCreation.CreateIndexes(typeof(All_Groups).Assembly, documentStore);

            return documentStore;
        }

        internal static IDocumentStore StartRaven()
        {
            dir = new Process
            {
                StartInfo =
                    {
                        FileName = "CMD.EXE",
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        RedirectStandardInput = true,
                        Arguments = @"/K C:\Projects\bowerbird.web\Src\packages\RavenDB.1.0.665-Unstable\server\Raven.Server.exe -ram",
                        RedirectStandardOutput = true
                    }
            };

            dir.Start();
            sw = dir.StandardInput;
            sr = dir.StandardOutput;
            sw.AutoFlush = true;
            
            //sw.WriteLine(@"C:\Projects\bowerbird.web\Src\packages\RavenDB.1.0.665-Unstable\server\Raven.Server.exe -ram");

            return RamDocumentStore();
        }

        internal static void KillRaven()
        {
            sw.WriteLine("q");
            sw.Close();
            sr.Close();
        }
    }
}