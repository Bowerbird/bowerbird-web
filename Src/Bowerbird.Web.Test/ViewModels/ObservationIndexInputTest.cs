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

namespace Bowerbird.Web.Test.ViewModels
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ObservationIndexInputTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize()
        {
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

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationIndexInput_UserId_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationIndexInput() {UserId = FakeValues.UserId}.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationIndexInput_ObservationId_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationIndexInput() { ObservationId = FakeValues.KeyString }.ObservationId);
        }

        #endregion

        #region Method tests

        #endregion
    }
}