/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.MediaResources;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Extensions;
using Raven.Client.Indexes;

namespace Bowerbird.Test.Utils
{
    public class DocumentStoreHelper
    {
        public const string TestDb = "bowerbird_test";
        public const string DevDb = "bowerbird_dev";

        public static IDocumentStore InMemoryDocumentStore(bool createIndexes = true)
        {
            var documentStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true,
                Conventions = new DocumentConvention()
                                  {
                                      DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites
                                  }
            }
            .Initialize();

            if (createIndexes) IndexCreation.CreateIndexes(typeof(All_Members).Assembly, documentStore);

            return documentStore;
        }

        public static IDocumentStore ServerDocumentStore(bool createIndexes = true)
        {
            var documentStore = new DocumentStore { Url = "http://zen:8080/", DefaultDatabase = TestDb};

            documentStore.Conventions.FindIdentityProperty =
                                prop =>
                                    // My custom ID for a given class.
                                    //(prop.DeclaringType.IsSubclassOf(typeof(DomainModelWithId)) && prop.Name == "Id")
                                    //(prop.DeclaringType == typeof(Role) && prop.Name == "Id")
                                    //|| (prop.DeclaringType == typeof(Permission) && prop.Name == "Id")
                                    // Default to general purpose.
                                    //prop.Name == "Id";
                                    prop.Name == "Id";

            documentStore.Initialize();

            documentStore.DatabaseCommands.EnsureDatabaseExists(TestDb);

            if(createIndexes)IndexCreation.CreateIndexes(typeof(All_Members).Assembly, documentStore);

            // remove all records from server before running test
            using(var session = documentStore.OpenSession())
            {
                session.DeleteFromDb(session.Query<User>());
                session.DeleteFromDb(session.Query<Watchlist>());
                session.DeleteFromDb(session.Query<Team>());
                session.DeleteFromDb(session.Query<Post>());
                session.DeleteFromDb(session.Query<Role>());
                session.DeleteFromDb(session.Query<Project>());
                session.DeleteFromDb(session.Query<Permission>());
                session.DeleteFromDb(session.Query<Organisation>());
                session.DeleteFromDb(session.Query<Observation>());
                session.DeleteFromDb(session.Query<ObservationNote>());
                session.DeleteFromDb(session.Query<Comment>());
                session.DeleteFromDb(session.Query<Member>());
                session.DeleteFromDb(session.Query<GroupMember>());
                session.DeleteFromDb(session.Query<GlobalMember>());
                session.DeleteFromDb(session.Query<MediaResource>());
                session.DeleteFromDb(session.Query<ImageMediaResource>());
                session.DeleteFromDb(session.Query<OtherMediaResource>());

                session.SaveChanges();
            }

            return documentStore;
        }
    }
}