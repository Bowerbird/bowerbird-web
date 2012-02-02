/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Comments;
using Bowerbird.Core.DomainModels.MediaResources;

namespace Bowerbird.Test.Utils
{
    #region Namespaces

    using System.Collections.Generic;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    public class FakeObjects
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
                new List<MediaResource>()
                );

            ((IAssignableId)observation).SetIdTo("observations", id);

            return observation;
        }

        public static MediaResource TestImageMediaResourceWithId()
        {
            return TestImageMediaResourceWithId(FakeValues.KeyString);
        }

        public static MediaResource TestImageMediaResourceWithId(string id)
        {
            var imageMediaResource = new ImageMediaResource(
                TestUser(),
                FakeValues.CreatedDateTime,
                FakeValues.Filename,
                FakeValues.FileFormat,
                FakeValues.Description,
                FakeValues.Number,
                FakeValues.Number
                );

            ((IAssignableId)imageMediaResource).SetIdTo("mediaresources", id);

            return imageMediaResource;

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
                new List<MediaResource>() { TestImageMediaResourceWithId() }
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

        public static ObservationComment TestObservationComment()
        {
            return new ObservationComment(
                TestUserWithId(),
                TestObservationWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Comment
                );
        }

        public static ObservationComment TestObservationCommentWithId()
        {
            return TestObservationCommentWithId(FakeValues.KeyString);
        }

        public static ObservationComment TestObservationCommentWithId(string id)
        {
            var observationComment = new ObservationComment(
                TestUserWithId(),
                TestObservationWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Comment
                );

            ((IAssignableId)observationComment).SetIdTo("comments", id);

            return observationComment;
        }

        public static PostComment TestPostComment()
        {
            return new PostComment(
                TestUserWithId(),
                TestPostWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Message
                );
        }

        public static PostComment TestPostCommentWithId()
        {
            return TestPostCommentWithId(FakeValues.KeyString);
        }

        public static PostComment TestPostCommentWithId(string id)
        {
            var postComment = TestPostComment();

            ((IAssignableId)postComment).SetIdTo("comments", id);

            return postComment;
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

    }
}