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

    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Extensions;

    #endregion

    public class ObservationNoteTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        const string additionalString = "_";

        private static IEnumerable<MediaResource> TestMediaResources()
        {
            return new List<MediaResource>()
            {
                new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description)
            };
        }

        private static Observation TestObservation()
        {
            return new Observation(
                FakeObjects.TestUser(),
                FakeValues.Title,
                FakeValues.CreatedDateTime,
                FakeValues.Latitude,
                FakeValues.Longitude,
                FakeValues.Address,
                FakeValues.IsTrue,
                FakeValues.Category,
                TestMediaResources()
                );
        }

        private static IDictionary<string,string> TestDescriptions()
        {
            return new Dictionary<string, string>() { { "a", "b" } };
        }

        private static IDictionary<string, string> TestReferences()
        {
            return new Dictionary<string, string>() { { "a", "b" } };
        }

        private static IDictionary<string, string> AnotherTestDescriptions()
        {
            return new Dictionary<string, string>() {{"c", "d"}};
        }

        private static IDictionary<string, string> AnotherTestReferences()
        {
            return new Dictionary<string, string>() { { "c", "d" } };
        }

        private static ObservationNote TestObservationNote()
        {
            return new ObservationNote(
                FakeObjects.TestUser(), 
                TestObservation(), 
                FakeValues.CommonName, 
                FakeValues.ScientificName,
                FakeValues.Taxonomy, 
                FakeValues.Tags, 
                TestDescriptions(), 
                TestReferences(),
                FakeValues.Notes,
                FakeValues.CreatedDateTime);
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Constructor_Sets_Property_Values()
        {
            var testObservation = TestObservationNote();

            Assert.AreEqual(testObservation.CommonName, FakeValues.CommonName);
            Assert.AreEqual(testObservation.ScientificName, FakeValues.ScientificName);
            Assert.AreEqual(testObservation.Taxonomy, FakeValues.Taxonomy);
            Assert.AreEqual(testObservation.Tags, FakeValues.Tags);
            Assert.AreEqual(testObservation.Notes, FakeValues.Notes);
            Assert.AreEqual(testObservation.Observation.Id, TestObservation().Id);
            Assert.AreEqual(testObservation.Observation.Title, TestObservation().Title);
            Assert.AreEqual(testObservation.User.FirstName, FakeObjects.TestUser().FirstName);
            Assert.AreEqual(testObservation.User.LastName, FakeObjects.TestUser().LastName);
            Assert.AreEqual(testObservation.User.Id, FakeObjects.TestUser().Id);
            Assert.AreEqual(testObservation.Descriptions, TestDescriptions());
            Assert.AreEqual(testObservation.References, TestReferences());
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails_Updates_Property_Values()
        {
            var testObservationNote = TestObservationNote();

            testObservationNote.UpdateDetails(
                FakeObjects.TestUser(), 
                FakeValues.CommonName.AppendWith(additionalString),
                FakeValues.ScientificName.AppendWith(additionalString),
                FakeValues.Taxonomy.AppendWith(additionalString),
                FakeValues.Tags.AppendWith(additionalString), 
                AnotherTestDescriptions(), 
                AnotherTestReferences(),
                FakeValues.Notes.AppendWith(additionalString));

            Assert.AreEqual(testObservationNote.CommonName, FakeValues.CommonName.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.ScientificName, FakeValues.ScientificName.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.Taxonomy, FakeValues.Taxonomy.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.Tags, FakeValues.Tags.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.Notes, FakeValues.Notes.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.Descriptions, AnotherTestDescriptions());
            Assert.AreEqual(testObservationNote.References, AnotherTestReferences());
        }

        #endregion
    }
}