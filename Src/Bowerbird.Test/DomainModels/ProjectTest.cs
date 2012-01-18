/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.DomainModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Extensions;

    #endregion

    public class ProjectTest
    {
        #region Test Infrastructure

        const string additionalString = "_";

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static Project TestProject()
        {
            return new Project(
                FakeObjects.TestUser(), 
                FakeValues.Name, 
                FakeValues.Description
                );
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Constructor_Populates_Properties_With_Values()
        {
            var testProject = new Project(FakeObjects.TestUser(), FakeValues.Name, FakeValues.Description);

            Assert.AreEqual(testProject.Name, FakeValues.Name);
            Assert.AreEqual(testProject.Description, FakeValues.Description);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_UpdateDetails_Populates_Properties_With_Values()
        {
            var testProject = TestProject();

            testProject.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Name.AppendWith(additionalString),
                FakeValues.Description.AppendWith(additionalString)
                );

            Assert.AreEqual(testProject.Name, FakeValues.Name.AppendWith(additionalString));
            Assert.AreEqual(testProject.Description, FakeValues.Description.AppendWith(additionalString));
        }

        #endregion
    }
}