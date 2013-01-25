/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

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

        public object BuildNewPost(string groupId)
        {
            return _postViewFactory.MakeNewPost(groupId);
        }

        public dynamic BuildPost(string id)
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

            var query = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupIds.Any(y => y == groupId) && x.ParentContributionType == "post");

            return ExecuteQuery(postsQueryInput, query);
        }

        public object BuildUserPostList(string userId, PostsQueryInput postsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(postsQueryInput, "postsQueryInput");

            var query = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.UserId == userId && x.ParentContributionType == "post");

            return ExecuteQuery(postsQueryInput, query);
        }

        public object BuildHomePostList(string userId, PostsQueryInput postsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(postsQueryInput, "postsQueryInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships.Select(x => x.Group.Id);

            var query = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupIds.Any(y => y.In(groupIds)) && x.ParentContributionType == "post");

            return ExecuteQuery(postsQueryInput, query);
        }

        private object ExecuteQuery(PostsQueryInput postsQueryInput, IRavenQueryable<All_Contributions.Result> query)
        {
            switch (postsQueryInput.Sort.ToLower())
            {
                default:
                case "newest":
                    query = query.OrderByDescending(x => x.CreatedDateTime);
                    break;
                case "oldest":
                    query = query.OrderBy(x => x.CreatedDateTime);
                    break;
            }

            RavenQueryStatistics stats;

            var authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            return query.Skip(postsQueryInput.GetSkipIndex())
                .Statistics(out stats)
                .Take(postsQueryInput.GetPageSize())
                .ToList()
                .Select(x => _postViewFactory.Make(x.Contribution as Post, x.User, x.Groups.First(), authenticatedUser))
                .ToPagedList(
                    postsQueryInput.Page,
                    postsQueryInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}