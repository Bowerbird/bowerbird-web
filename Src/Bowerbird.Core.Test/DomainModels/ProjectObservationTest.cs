/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DomainModels
{
    #region Namespaces

    using System;
    
    using NUnit.Framework;
    using Moq;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;

    #endregion

    public class ProjectObservationTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static ProjectObservation TestProjectObservation()
        {
            return new ProjectObservation(
                new Mock<User>().Object,
                DateTime.Now,
                new Mock<Project>().Object,
                new Mock<Observation>().Object
                );
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservation_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectObservation(null,DateTime.Now,new Mock<Project>().Object,new Mock<Observation>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservation_Constructor_Passing_Null_Project_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectObservation(new Mock<User>().Object,DateTime.Now,null,new Mock<Observation>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservation_Constructor_Passing_Null_Observation_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectObservation(new Mock<User>().Object,DateTime.Now,new Mock<Project>().Object,null)));
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservation_Project_Is_TypeOf_DenormalisedNamedModelReference_Of_Project()
        {
            Assert.IsInstanceOf<DenormalisedNamedDomainModelReference<Project>>(TestProjectObservation().Project);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservation_Observation_Is_TypeOf_DenormalisedObservationReference_Of_Project()
        {
            Assert.IsInstanceOf<DenormalisedObservationReference>(TestProjectObservation().Observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservation_User_Is_TypeOf_DenormalisedUserReference()
        {
            Assert.IsInstanceOf<DenormalisedUserReference>(TestProjectObservation().CreatedByUser);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservation_CreatedDateTime_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(TestProjectObservation().CreatedDateTime);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}