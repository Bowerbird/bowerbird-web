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
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using System;

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
    public class ObservationTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private Observation TestObservation()
        {
            return new Observation(
                    FakeObjects.TestUserWithId(),
                    FakeValues.Title,
                    FakeValues.CreatedDateTime,
                    FakeValues.CreatedDateTime,
                    FakeValues.Latitude,
                    FakeValues.Longitude,
                    FakeValues.Address,
                    FakeValues.IsTrue,
                    FakeValues.Category,
                    FakeObjects.TestUserProjectWithId(),
                    new List<Project>() { FakeObjects.TestProjectWithId() },
                    new List<Tuple<MediaResource, string, string>>()
                    );
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Constructor()
        {
            var observation = TestObservation();

            Assert.AreEqual(FakeObjects.TestUserWithId().DenormalisedUserReference(), observation.User);
            Assert.AreEqual(FakeValues.Title, observation.Title);
            Assert.AreEqual(FakeValues.CreatedDateTime, observation.CreatedOn);
            Assert.AreEqual(FakeValues.CreatedDateTime, observation.ObservedOn);
            Assert.AreEqual(FakeValues.Latitude, observation.Latitude);
            Assert.AreEqual(FakeValues.Longitude, observation.Longitude);
            Assert.AreEqual(FakeValues.Address, observation.Address);
            Assert.AreEqual(FakeValues.IsTrue, observation.IsIdentificationRequired);
            Assert.AreEqual(FakeValues.Category, observation.ObservationCategory);
            Assert.AreEqual(FakeValues.Title, observation.Title);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_UpdateDetails()
        {
            var observation = TestObservation();

            observation.UpdateDetails(
                FakeObjects.TestUserWithId(),
                FakeValues.Title.PrependWith("new"),
                FakeValues.ModifiedDateTime,
                FakeValues.Latitude.PrependWith("new"),
                FakeValues.Longitude.PrependWith("new"),
                FakeValues.Address.PrependWith("new"),
                FakeValues.IsFalse,
                FakeValues.Category.PrependWith("new")
                );

            Assert.AreEqual(FakeValues.Title.PrependWith("new"), observation.Title);
            Assert.AreEqual(FakeValues.ModifiedDateTime, observation.ObservedOn);
            Assert.AreEqual(FakeValues.Latitude.PrependWith("new"), observation.Latitude);
            Assert.AreEqual(FakeValues.Longitude.PrependWith("new"), observation.Longitude);
            Assert.AreEqual(FakeValues.Address.PrependWith("new"), observation.Address);
            Assert.AreEqual(FakeValues.IsFalse, observation.IsIdentificationRequired);
            Assert.AreEqual(FakeValues.Category.PrependWith("new"), observation.ObservationCategory);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_AddNote()
        {
            var observation = TestObservation();

            var observationNote = FakeObjects.TestObservationNoteWithId();
            observation.AddNote(observationNote);

            Assert.AreEqual(observation.Notes.ToList()[0], observationNote);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_RemoveNote()
        {
            var observation = TestObservation();

            var observationNote = FakeObjects.TestObservationNoteWithId();
            observation.AddNote(observationNote);

            Assert.AreEqual(observation.Notes.ToList()[0], observationNote);

            observation.RemoveNote(observationNote.Id);

            Assert.IsTrue(observation.Notes.Count() == 0);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_AddGroup()
        {
            var observation = TestObservation();
            var createdDateTime = DateTime.UtcNow;
            var group = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();

            observation.AddGroup(
                group,
                user,
                createdDateTime
                );

            // group 2 because all observations have the user's project as the first group
            var observationGroup = observation.Groups.ToList()[1];

            Assert.AreEqual(group.Id, observationGroup.GroupId);
            Assert.AreEqual(user.DenormalisedUserReference(), observationGroup.User);
            // > to pass the following assertion, database needs to be cleaned out before each test
            //Assert.AreEqual(createdDateTime, observationGroup.CreatedDateTime);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_RemoveGroup()
        {
            var observation = TestObservation();
            var createdDateTime = DateTime.UtcNow;
            var group = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();

            observation.AddGroup(
                group,
                user,
                createdDateTime
                );

            Assert.IsTrue(observation.Groups.Count() == 1);

            observation.RemoveGroup(group.Id);

            Assert.IsTrue(observation.Groups.Count() == 0);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_AddMedia()
        {
            var observation = TestObservation();
            var mediaResource = FakeObjects.TestMediaResourceWithId();

            observation.AddMedia(
                mediaResource,
                FakeValues.Description,
                FakeValues.Description
                );

            var observationMedia = observation.Media.ToList()[0];

            Assert.AreEqual(mediaResource, observationMedia.MediaResource);
            Assert.AreEqual(FakeValues.Description, observationMedia.Licence);
            Assert.AreEqual(FakeValues.Description, observationMedia.Description);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_RemoveMedia()
        {
            var observation = TestObservation();
            var mediaResource = FakeObjects.TestMediaResourceWithId();

            observation.AddMedia(
                mediaResource,
                FakeValues.Description,
                FakeValues.Description
                );

            var observationMedia = observation.Media.ToList()[0];

            observation.RemoveMedia(mediaResource.Id);

            Assert.IsTrue(observation.Media.ToList().Count == 0);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_AddComment()
        {
            var observation = TestObservation();
            var comment = FakeObjects.TestCommentWithId();
            var user = FakeObjects.TestUserWithId();
            var createdDateTime = DateTime.UtcNow;

            observation.AddComment(
                comment.Message,
                user,
                createdDateTime
                );

            Assert.IsTrue(observation.Discussion.Comments.ToList().Count == 1);
            Assert.AreEqual(comment.Message, observation.Discussion.Comments.ToList()[0].Message);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_RemoveComment()
        {
            var observation = TestObservation();
            var comment = FakeObjects.TestCommentWithId();
            var user = FakeObjects.TestUserWithId();
            var createdDateTime = DateTime.UtcNow;

            observation.AddComment(
                comment.Message,
                user,
                createdDateTime
                );

            observation.RemoveComment(comment.Id);

            Assert.IsTrue(observation.Discussion.Comments.ToList().Count == 0);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_UpdateComment()
        {
            var observation = TestObservation();
            var comment = FakeObjects.TestCommentWithId();
            var user = FakeObjects.TestUserWithId();
            var createdDateTime = DateTime.UtcNow.AddDays(-1);
            var modifiedDateTime = DateTime.UtcNow;

            observation.AddComment(
                comment.Message,
                user,
                createdDateTime
                );

            observation.UpdateComment(
                comment.Id,
                FakeValues.Comment.AppendWith("new"),
                user,
                modifiedDateTime
                );

            var updatedComment = observation.Discussion.Comments.ToList()[0];

            Assert.AreEqual(modifiedDateTime, updatedComment.EditedOn);
            Assert.AreEqual(FakeValues.Comment.AppendWith("new"), updatedComment.Message);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_GetPrimaryImage()
        {
            var observation = TestObservation();
            var mediaResource1 = FakeObjects.TestMediaResourceWithId();
            var mediaResource2 = FakeObjects.TestMediaResourceWithId("12345");

            observation.AddMedia(
                mediaResource1,
                FakeValues.Description,
                FakeValues.Description
                );

            observation.AddMedia(
                mediaResource2,
                FakeValues.Description,
                FakeValues.Description
                );

            var observationMedia = observation.GetPrimaryImage();

            Assert.AreEqual(mediaResource1, observationMedia.MediaResource);
        }

        #endregion
    }
}