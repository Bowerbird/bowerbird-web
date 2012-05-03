/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Factories;
using Bowerbird.Core.Queries;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Web.ViewModelBuilders
{
    [TestFixture]
    public class TeamsViewModelBuilderTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;

            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Property tests

        #endregion

        #region Constructor tests

        [Test]
        public void TeamsQuery_Constructor_Passed_Null_AvatarFactory()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new TeamsViewModelBuilder(
                        new Mock<IUserContext>().Object,
                        new Mock<IDocumentSession>().Object,
                        new Mock<IUsersGroupsQuery>().Object,
                        null)));
        }

        [Test]
        public void TeamsQuery_Constructor_Passed_Null_UsersGroupsQuery()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new TeamsViewModelBuilder(
                        new Mock<IUserContext>().Object,
                        new Mock<IDocumentSession>().Object,
                        null,
                        new Mock<IAvatarFactory>().Object
                              )));
        }

        [Test]
        public void TeamsQuery_Constructor_Passed_Null_DocumentSession()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new TeamsViewModelBuilder(
                        new Mock<IUserContext>().Object,
                        null,
                        new Mock<IUsersGroupsQuery>().Object,
                        new Mock<IAvatarFactory>().Object
                              )));
        }

        [Test]
        public void TeamsQuery_Constructor_Passed_Null_UserContext()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new TeamsViewModelBuilder(
                        null,
                        new Mock<IDocumentSession>().Object,
                        new Mock<IUsersGroupsQuery>().Object,
                        new Mock<IAvatarFactory>().Object
                              )));
        }

        #endregion

        #region Methods tests

        [Test]
        public void TeamsQuery_TeamsHavingAddProjectPermission()
        {
            using(var session = _documentStore.OpenSession())
            {
                var teamsViewModelBuilder = new TeamsViewModelBuilder(
                    new Mock<IUserContext>().Object,
                    session,
                    new Mock<IUsersGroupsQuery>().Object,
                    new Mock<IAvatarFactory>().Object
                );
            }
        }

        #endregion
    }
}