/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using NUnit.Framework;
using Bowerbird.Test.Utils;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
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

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Constructor()
        {
            var testObservation = new ObservationNote(
                FakeObjects.TestUserWithId(),
                FakeObjects.TestObservationWithId(),
                FakeValues.CommonName,
                FakeValues.ScientificName,
                FakeValues.Taxonomy,
                FakeValues.Tags,
                new Dictionary<string, string>() { { "a", "b" } },
                new Dictionary<string, string>() { { "c", "d" } },
                FakeValues.CreatedDateTime);

            Assert.AreEqual(testObservation.CommonName, FakeValues.CommonName);
            Assert.AreEqual(testObservation.ScientificName, FakeValues.ScientificName);
            Assert.AreEqual(testObservation.Taxonomy, FakeValues.Taxonomy);
            Assert.AreEqual(testObservation.Tags, FakeValues.Tags);
            Assert.AreEqual(testObservation.Observation.Id, FakeObjects.TestObservationWithId().Id);
            Assert.AreEqual(testObservation.User.FirstName, FakeObjects.TestUserWithId().FirstName);
            Assert.AreEqual(testObservation.User.LastName, FakeObjects.TestUserWithId().LastName);
            Assert.AreEqual(testObservation.User.Id, FakeObjects.TestUserWithId().Id);
            Assert.AreEqual(testObservation.Descriptions, new Dictionary<string, string>() { { "a", "b" } });
            Assert.AreEqual(testObservation.References, new Dictionary<string, string>() { { "c", "d" } });
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails()
        {
            var testObservationNote = new ObservationNote(
                FakeObjects.TestUser(),
                FakeObjects.TestObservationWithId(),
                FakeValues.CommonName,
                FakeValues.ScientificName,
                FakeValues.Taxonomy,
                FakeValues.Tags,
                new Dictionary<string, string>() { { "a", "b" } },
                new Dictionary<string, string>() { { "c", "d" } },
                FakeValues.CreatedDateTime);

            testObservationNote.UpdateDetails(
                FakeObjects.TestUser(), 
                FakeValues.CommonName.AppendWith(additionalString),
                FakeValues.ScientificName.AppendWith(additionalString),
                FakeValues.Taxonomy.AppendWith(additionalString),
                FakeValues.Tags.AppendWith(additionalString),
                new Dictionary<string, string>() { { "e", "f" } },
                new Dictionary<string, string>() { { "g", "h" } }
                );

            Assert.AreEqual(testObservationNote.CommonName, FakeValues.CommonName.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.ScientificName, FakeValues.ScientificName.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.Taxonomy, FakeValues.Taxonomy.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.Tags, FakeValues.Tags.AppendWith(additionalString));
            Assert.AreEqual(testObservationNote.Descriptions, new Dictionary<string, string>() { { "e", "f" } });
            Assert.AreEqual(testObservationNote.References, new Dictionary<string, string>() { { "g", "h" } });
        }

        #endregion
    }
}