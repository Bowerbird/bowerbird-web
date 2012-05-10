/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Factories;
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
            Assert.False(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new TeamsViewModelBuilder(
                        new Mock<IDocumentSession>().Object,
                        new Mock<ITeamViewFactory>().Object))
                        );
        }

       // TODO: Add fail tests... 

        #endregion
    }
}