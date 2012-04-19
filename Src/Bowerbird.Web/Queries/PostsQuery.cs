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
using Bowerbird.Core.Paging;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Queries
{
    public class PostsQuery : IPostsQuery
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IUsersGroupsQuery _usersGroupsQuery;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public PostsQuery(
            IUserContext userContext,
            IDocumentSession documentSession,
            IUsersGroupsQuery usersGroupsQuery,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(usersGroupsQuery, "usersGroupsQuery");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _userContext = userContext;
            _documentSession = documentSession;
            _usersGroupsQuery = usersGroupsQuery;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Methods

        public PostList MakePostList(PostListInput listInput)
        {
            RavenQueryStatistics stats;

            var posts = _documentSession
                .Query<Post>()
                .Where(x => x.GroupId == listInput.GroupId)
                .Include(x => x.GroupId)
                .OrderByDescending(x => x.CreatedOn)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            return new PostList
            {
                GroupId = listInput.GroupId,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Posts = posts.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}