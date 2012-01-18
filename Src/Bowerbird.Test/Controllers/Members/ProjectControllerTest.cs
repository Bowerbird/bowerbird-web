/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Test.Utils;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Members
{
    #region Namespaces

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core;
    using Bowerbird.Core.Tasks;
    using Bowerbird.Web.Controllers.Members;
    using Bowerbird.Web.Config;

    #endregion

    [TestFixture]
    public class ProjectControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserTasks> _mockUserTasks;
        private Mock<IUserContext> _mockUserContext;
        private ProjectController _controller;
        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserTasks = new Mock<IUserTasks>();
            _mockUserContext = new Mock<IUserContext>();
            _controller = new ProjectController(
                _mockCommandProcessor.Object,
                _mockUserTasks.Object,
                _mockUserContext.Object,
                _documentStore.OpenSession());
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        #endregion 
    }
}