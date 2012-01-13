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
    using Bowerbird.Core.DomainModels;

    #endregion

    [TestFixture]
    public class ObservationIndexTest
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
        public void ObservationIndex_Observation_Is_TypeOf_Observation()
        {
            Assert.IsInstanceOf<Observation>(new ObservationIndex() {Observation = FakeObjects.TestObservation()}.Observation);
        }

        #endregion

        #region Method tests

        #endregion
    }
}