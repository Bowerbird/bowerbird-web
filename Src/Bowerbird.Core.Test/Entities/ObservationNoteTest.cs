/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Entities
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Entities.DenormalisedReferences;
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
                FakeValues.Notes);
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationNote(null, TestObservation(), FakeValues.CommonName, FakeValues.ScientificName, FakeValues.Taxonomy, FakeValues.Tags, TestDescriptions(), TestReferences(), FakeValues.Notes)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Constructor_Passing_Null_Observation_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationNote(FakeObjects.TestUser(), null, FakeValues.CommonName, FakeValues.ScientificName, FakeValues.Taxonomy, FakeValues.Tags, TestDescriptions(), TestReferences(), FakeValues.Notes)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Constructor_Passing_Null_Descriptions_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationNote(FakeObjects.TestUser(), TestObservation(), FakeValues.CommonName, FakeValues.ScientificName, FakeValues.Taxonomy, FakeValues.Tags, null, TestReferences(), FakeValues.Notes)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Constructor_Passing_Null_References_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationNote(FakeObjects.TestUser(), TestObservation(), FakeValues.CommonName, FakeValues.ScientificName, FakeValues.Taxonomy, FakeValues.Tags, TestDescriptions(), null, FakeValues.Notes)));
        }

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
            Assert.AreEqual(testObservation.User.Email, FakeObjects.TestUser().Email);
            Assert.AreEqual(testObservation.User.Id, FakeObjects.TestUser().Id);
            Assert.AreEqual(testObservation.Descriptions, TestDescriptions());
            Assert.AreEqual(testObservation.References, TestReferences());
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_User_Is_OfType_DenormalisedUserReference()
        {
            Assert.IsInstanceOf<DenormalisedUserReference>(TestObservationNote().User);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Observation_Is_OfType_DenormalisedObservationReference()
        {
            Assert.IsInstanceOf<DenormalisedObservationReference>(TestObservationNote().Observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Descriptions_Is_OfType_Dictionary_Of_String_String()
        {
            Assert.IsInstanceOf<Dictionary<string,string>>(TestObservationNote().Descriptions);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_References_Is_OfType_Dictionary_Of_String_String()
        {
            Assert.IsInstanceOf<Dictionary<string, string>>(TestObservationNote().References);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_CommonName_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestObservationNote().CommonName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_ScientificName_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestObservationNote().ScientificName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Taxonomy_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestObservationNote().Taxonomy);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Tags_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestObservationNote().Tags);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Notes_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestObservationNote().Notes);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_SubmittedOn_Is_OfType_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(TestObservationNote().SubmittedOn);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails_Passing_Null_User_Throws_DesignByContractException()
        {
            var testObservationNote = TestObservationNote();

            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => testObservationNote.UpdateDetails(null, FakeValues.CommonName, FakeValues.ScientificName, FakeValues.Taxonomy, FakeValues.Tags, TestDescriptions(), TestReferences(), FakeValues.Notes)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails_Passing_Null_Descriptions_Throws_DesignByContractException()
        {
            var testObservationNote = TestObservationNote();

            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => testObservationNote.UpdateDetails(FakeObjects.TestUser(), FakeValues.CommonName, FakeValues.ScientificName, FakeValues.Taxonomy, FakeValues.Tags, null, TestReferences(), FakeValues.Notes)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails_Passing_Null_References_Throws_DesignByContractException()
        {
            var testObservationNote = TestObservationNote();

            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => testObservationNote.UpdateDetails(FakeObjects.TestUser(), FakeValues.CommonName, FakeValues.ScientificName, FakeValues.Taxonomy, FakeValues.Tags, TestDescriptions(), null, FakeValues.Notes)));
        }

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