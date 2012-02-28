/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;

namespace Bowerbird.Test.Utils
{
    #region Namespaces

    using System.Collections.Generic;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    public static class FakeObjects
    {
        public static User TestUser()
        {
            return new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                TestRoles()
                )
                .UpdateLastLoggedIn()
                .IncrementFlaggedItemsOwned()
                .IncrementFlagsRaised();
        }

        public static User TestUserWithId()
        {
            return TestUserWithId(FakeValues.KeyString);
        }

        public static User TestUserWithId(string id)
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                TestRoles()
                )
                .UpdateLastLoggedIn()
                .IncrementFlaggedItemsOwned()
                .IncrementFlagsRaised();

            ((IAssignableId)user).SetIdTo("users", id);

            return user;
        }

        public static IEnumerable<Role> TestRoles()
        {
            return new List<Role>()
            {
                new Role
                (
                    "Member",
                    "Member role",
                    "Member description",
                    TestPermissions()
                )
            };
        }

        public static IEnumerable<Permission> TestPermissions()
        {
            return new List<Permission>
            {
                new Permission("Read", "Read permission", "Read description"),
                new Permission("Write", "Write permission", "Write description")
            };
        }
        
        public static Project TestProject()
        {
            return new Project(TestUser(), FakeValues.Name, FakeValues.Description, FakeValues.Website);
        }

        public static Project TestProjectWithId()
        {
            return TestProjectWithId(FakeValues.KeyString);
        }

        public static Project TestProjectWithId(string id)
        {
            var project = new Project(
                TestUser(),
                FakeValues.Name,
                FakeValues.Description,
                FakeValues.Website);

            ((IAssignableId)project).SetIdTo("projects", id);

            return project;
        }

        public static Team TestTeam()
        {
            return new Team(
                TestUser(), 
                FakeValues.Name, 
                FakeValues.Description, 
                FakeValues.Website);
        }

        public static Team TestTeamWithId()
        {
            return TestTeamWithId(FakeValues.KeyString);
        }

        public static Team TestTeamWithId(string id)
        {
            var team = new Team(
                TestUser(),
                FakeValues.Name,
                FakeValues.Description,
                FakeValues.Website);

            ((IAssignableId)team).SetIdTo("teams", id);

            return team;
        }

        public static GroupMember TestGroupMember()
        {
            return new GroupMember(
                TestUserWithId(),
                TestProjectWithId(),
                TestUserWithId(),
                TestRoles());
        }

        public static GroupMember TestGroupMemberWithId()
        {
            var projectMember = TestGroupMember();

            ((IAssignableId)projectMember).SetIdTo("members", FakeValues.KeyString);

            return projectMember;
        }

        public static GroupMember TestGroupMemberWithId(string id)
        {
            var projectMember = TestGroupMember();

            ((IAssignableId)projectMember).SetIdTo("members", id);

            return projectMember;
        }

        public static Observation TestObservationWithId()
        {
            return TestObservationWithId(FakeValues.KeyString);
        }

        public static Observation TestObservationWithId(string id)
        {
            var observation = new Observation(
                TestUserWithId(),
                FakeValues.Title,
                FakeValues.CreatedDateTime,
                FakeValues.CreatedDateTime,
                FakeValues.Latitude,
                FakeValues.Longitude,
                FakeValues.Address,
                FakeValues.IsTrue,
                FakeValues.Category,
                new Dictionary<MediaResource, string>() { { TestImageMediaResourceWithId("1"), FakeValues.Description } }
                );

            ((IAssignableId)observation).SetIdTo("observations", id);

            return observation;
        }

        public static MediaResource TestImageMediaResourceWithId()
        {
            return TestMediaResourceWithId(FakeValues.KeyString);
        }

        public static MediaResource TestImageMediaResourceWithId(string id)
        {
            return TestMediaResourceWithId(id);
        }

        public static Organisation TestOrganisation()
        {
            return new Organisation(
                TestUserWithId(),
                FakeValues.Name,
                FakeValues.Description,
                FakeValues.Website
                );
        }

        public static Organisation TestOrganisationWithId()
        {
            return TestOrganisationWithId(FakeValues.KeyString);
        }

        public static Organisation TestOrganisationWithId(string id)
        {
            var organisation = TestOrganisation();

            ((IAssignableId)organisation).SetIdTo("organisations", id);

            return organisation;
        }

        public static Post TestPost()
        {
            return new Post(
                TestUserWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Subject,
                FakeValues.Message,
                new List<MediaResource>() { TestImageMediaResourceWithId() },
                TestProjectWithId()
                );
        }

        public static Post TestPostWithId()
        {
            return TestPostWithId(FakeValues.KeyString);
        }

        public static Post TestPostWithId(string id)
        {
            var organisationPost = TestPost();

            ((IAssignableId)organisationPost).SetIdTo("posts", id);

            return organisationPost;
        }

        public static Comment TestComment()
        {
            return new Comment(
                FakeValues.KeyLong.ToString(),
                TestUserWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Comment
                );
        }

        public static Comment TestCommentWithId()
        {
            return TestCommentWithId(FakeValues.KeyString);
        }

        public static Comment TestCommentWithId(string id)
        {
            var comment = new Comment(
                FakeValues.KeyLong.ToString(),
                TestUserWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Comment
                );

            ((IAssignableId)comment).SetIdTo("comments", id);

            return comment;
        }

        public static ObservationNote TestObservationNote()
        {
            return new ObservationNote(
                TestUserWithId(),
                TestObservationWithId(),
                FakeValues.CommonName,
                FakeValues.ScientificName,
                FakeValues.Taxonomy,
                FakeValues.Tags,
                new Dictionary<string, string>(){{FakeValues.Description,FakeValues.Description}}, 
                new Dictionary<string, string>(){{FakeValues.Description,FakeValues.Description}}, 
                FakeValues.Notes,
                FakeValues.CreatedDateTime
                );
        }

        public static ObservationNote TestObservationNoteWithId()
        {
            return TestObservationNoteWithId(FakeValues.KeyString);
        }

        public static ObservationNote TestObservationNoteWithId(string id)
        {
            var testObservationNote = TestObservationNote();

            ((IAssignableId) testObservationNote).SetIdTo("observationnotes", id);

            return testObservationNote;
        }

        public static StreamItem TestStreamItem(DomainModel model, string parentId = null)
        {
            return new StreamItem(
                TestUserWithId(),
                FakeValues.CreatedDateTime,
                model.GetType().ToString(), 
                model.Id,
                model,
                parentId
                );
        }

        public static Watchlist TestWatchlistWithId(string id)
        {
            var watchlist = TestWatchlist();

            ((IAssignableId)watchlist).SetIdTo("watchlists", id);

            return watchlist;
        }

        public static Watchlist TestWatchlistWithId()
        {
            return TestWatchlistWithId(FakeValues.KeyString);
        }

        public static Watchlist TestWatchlist()
        {
            return new Watchlist(
                TestUserWithId(), 
                FakeValues.Name, 
                FakeValues.QuerystringJson);
        }

        public static GroupAssociation TestGroupAssociation()
        {
            return new GroupAssociation(
                TestTeamWithId(),
                TestUserWithId(),
                FakeValues.CreatedDateTime
                );
        }

        public static MediaResource TestMediaResource()
        {
            return new MediaResource(
                FakeValues.MediaType,
                TestUserWithId(),
                FakeValues.CreatedDateTime,
                new Dictionary<string, string>() { { FakeValues.Description, FakeValues.Description } });
        }

        public static MediaResource TestMediaResourceWithId()
        {
            var mediaResource = TestMediaResource();

            ((IAssignableId)mediaResource).SetIdTo("mediaresources", FakeValues.KeyString);

            return mediaResource;
        }

        public static MediaResource TestMediaResourceWithId(string id)
        {
            var mediaResource = TestMediaResource();

            ((IAssignableId)mediaResource).SetIdTo("mediaresources", id);

            return mediaResource;
        }
    }
}