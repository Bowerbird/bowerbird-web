/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using NUnit.Framework;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Test.Utils;

namespace Bowerbird.Test.DomainModels.DenormalisedReferences
{
    [TestFixture]
    public class DenormalisedNamedDomainModelReferenceTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static DenormalisedNamedDomainModelReference<T> TestDenormalise<T>(T t) where T : INamedDomainModel
        {
            DenormalisedNamedDomainModelReference<T> denormalisedObservationReference = t;

            return denormalisedObservationReference;
        }
        
        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedNamedDomainModelReference_Implicit_Operator_Passing_Project()
        {
            var normalisedProject = FakeObjects.TestProject();

            DenormalisedNamedDomainModelReference<Project> denormalisedProject = TestDenormalise(normalisedProject);

            Assert.AreEqual(denormalisedProject.Id, normalisedProject.Id);
            Assert.AreEqual(denormalisedProject.Name, normalisedProject.Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedNamedDomainModelReference_Implicit_Operator_Passing_Team()
        {
            var normalisedTeam = FakeObjects.TestTeam();

            DenormalisedNamedDomainModelReference<Team> denormalisedTeam = TestDenormalise(normalisedTeam);

            Assert.AreEqual(denormalisedTeam.Id, normalisedTeam.Id);
            Assert.AreEqual(denormalisedTeam.Name, normalisedTeam.Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedNamedDomainModelReference_Implicit_Operator_Passing_Null()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => TestDenormalise<Project>(null)));
        }

        #endregion
    }
}