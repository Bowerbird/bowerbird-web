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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.Builders
{
    public class PostViewModelBuilder : IPostViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IPostViewFactory _postViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public PostViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IPostViewFactory postViewFactory,
            IUserContext userContext
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(postViewFactory, "postViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _postViewFactory = postViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Methods

        public object BuildCreatePost(string groupId)
        {
            return _postViewFactory.MakeCreatePost(groupId);
        }

        public object BuildPost(string id)
        {
            var authenticatedUser = _userContext.IsUserAuthenticated() ? _documentSession.Load<User>(_userContext.GetAuthenticatedUserId()) : null;

            var result = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ParentContributionId == id && x.ParentContributionType == "post")
                .First();

            var post = result.Contribution as Post;
            var group = result.Groups.First();
            var user = result.User;

            dynamic viewModel = _postViewFactory.Make(post, user, group, authenticatedUser);

            return viewModel;
        }

        public object BuildGroupPostList(string groupId, PostsQueryInput postsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(postsQueryInput, "postsQueryInput");

            return ExecuteQuery(postsQueryInput, new[] { groupId });
        }

        public object BuildHomePostList(string userId, PostsQueryInput postsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(postsQueryInput, "postsQueryInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships.Select(x => x.Group.Id);

            return ExecuteQuery(postsQueryInput, groupIds);
        }

        private object ExecuteQuery(PostsQueryInput postsQueryInput, IEnumerable<string> groupIds)
        {
            RavenQueryStatistics stats;
            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Contributions.Result, All_Contributions>()
                .Statistics(out stats)
                .SelectFields<All_Contributions.Result>("GroupIds", "CreatedDateTime", "ParentContributionId", "SubContributionId", "ParentContributionType", "SubContributionType", "UserId", "Observation", "Record", "Post", "User")
                .WhereIn("GroupIds", groupIds)
                .AndAlso()
                .WhereIn("ParentContributionType", new[] { "post" })
                .AndAlso()
                .WhereEquals("SubContributionType", null);

            if (!string.IsNullOrWhiteSpace(postsQueryInput.Query))
            {
                var field = "PostAllFields";

                if (postsQueryInput.Field.ToLower() == "title")
                {
                    field = "PostTitle";
                }
                if (postsQueryInput.Field.ToLower() == "body")
                {
                    field = "PostMessage";
                }

                query = query
                    .AndAlso()
                    .Search(field, postsQueryInput.Query);
            }

            switch (postsQueryInput.Sort.ToLower())
            {
                default:
                case "newest":
                    query = query.AddOrder(x => x.CreatedDateTime, true).AddOrder(x => x.PostTitle, false);
                    //query = query.OrderByDescending(x => x.CreatedDateTime);
                    break;
                case "oldest":
                    query = query.AddOrder(x => x.CreatedDateTime, false).AddOrder(x => x.PostTitle, false);
                    //query = query.OrderBy(x => x.CreatedDateTime);
                    break;
            }

            return query
                .Skip(postsQueryInput.GetSkipIndex())
                .Take(postsQueryInput.GetPageSize())
                .Select(x => _postViewFactory.Make(x.Contribution as Post, x.User, x.Groups.First(), authenticatedUser))
                .ToList()
                .ToPagedList(
                    postsQueryInput.GetPage(),
                    postsQueryInput.GetPageSize(),
                    stats.TotalResults
                );
        }

        #endregion
    }
}