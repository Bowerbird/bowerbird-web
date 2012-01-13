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

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ProjectObservationDeleteInputTest
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
        public void ProjectObservationDeleteInput_ProjectId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectObservationDeleteInput() { ProjectId = FakeValues.KeyString }.ProjectId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationDeleteInput_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectObservationDeleteInput() { UserId = FakeValues.KeyString }.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationDeleteInput_ObservationId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectObservationDeleteInput() { ObservationId = FakeValues.KeyString }.ObservationId);
        }

        #endregion

        #region Method tests

        #endregion
    }
}