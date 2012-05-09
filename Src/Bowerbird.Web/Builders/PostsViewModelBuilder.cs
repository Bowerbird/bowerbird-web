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
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Repositories;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Builders
{
    public class PostsViewModelBuilder : IPostsViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IPostViewFactory _postViewFactory;
        private readonly IStreamItemFactory _streamItemFactory;

        #endregion

        #region Constructors

        public PostsViewModelBuilder(
            IDocumentSession documentSession,
            IPostViewFactory postViewFactory,
            IStreamItemFactory streamItemFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(postViewFactory, "postViewFactory");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");

            _documentSession = documentSession;
            _postViewFactory = postViewFactory;
            _streamItemFactory = streamItemFactory;
        }

        #endregion

        #region Methods

        public object BuildPost(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _postViewFactory.Make(_documentSession.Load<Post>(idInput.Id));
        }

        public object BuildUserPostList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var posts = _documentSession
                .Query<Post>()
                .Where(x => x.User.Id == pagingInput.Id)
                .Include(x => x.GroupId)
                .OrderByDescending(x => x.CreatedOn)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _postViewFactory.Make(x));

            return new
            {
                pagingInput.Id,
                pagingInput.Page,
                pagingInput.PageSize,
                List = posts.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildGroupPostList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var posts = _documentSession
                .Query<Post>()
                .Where(x => x.GroupId == pagingInput.Id)
                .Include(x => x.GroupId)
                .OrderByDescending(x => x.CreatedOn)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _postViewFactory.Make(x));

            return new
            {
                pagingInput.Id,
                pagingInput.Page,
                pagingInput.PageSize,
                List = posts.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildPostStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Include(x => x.GroupId)
                .Where(x => x.ContributionType.Equals("post") && x.GroupId == pagingInput.Id)
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .ToPagedList(pagingInput.Page, pagingInput.PageSize, stats.TotalResults)
                .PagedListItems
                .Select(MakeStreamItem);
        }

        private object MakeStreamItem(All_Contributions.Result groupContributionResult)
        {
            object item = null;
            string description = null;
            Group group = null;
            string postedToGroupType = null;

            switch (groupContributionResult.ContributionType)
            {
                case "Post":
                    item = _postViewFactory.Make(groupContributionResult.Post);
                    description = groupContributionResult.Post.User.FirstName + " added a post";
                    group = _documentSession.LoadGroupById(groupContributionResult.Post.GroupId, out postedToGroupType);
                    break;
            }

            return _streamItemFactory.Make(
                item,
                group,
                "post",
                groupContributionResult.GroupUser,
                groupContributionResult.GroupCreatedDateTime,
                description);
        }

        #endregion
    }
}