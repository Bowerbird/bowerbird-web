using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Storage.Esent;

namespace Bowerbird.Core.Test.Repositories
{
    [TestFixture]
    public abstract class RepositoryBaseTest
    {
     
        protected IDocumentStore _store;

        [SetUp]
        public void SetUp()
        {
            _store = new EmbeddableDocumentStore()
            {
                RunInMemory = true
            };

            _store.Initialize();
        }

        [TearDown]
        public void CleanUp()
        {
            if (_store != null) _store.Dispose();
        }

    }
}