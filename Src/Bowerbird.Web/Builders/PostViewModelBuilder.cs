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
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Services;
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

        #endregion

        #region Constructors

        public PostViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Methods

        public object BuildNewPost(string groupId)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");

            return new
            {
                Subject = string.Empty,
                Message = string.Empty,
                GroupId = groupId
            };
        }

        public object BuildPost(string postId)
        {
            Check.RequireNotNullOrWhitespace(postId, "postId");

            return MakePost(_documentSession.Load<Post>(postId));
        }

        public object BuildUserPostList(string userId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<Post>()
                .Where(x => x.User.Id == userId)
                .OrderByDescending(x => x.CreatedOn)
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakePost)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        public object BuildGroupPostList(string groupId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<Post>()
                .Where(x => x.GroupId == groupId)
                .Include(x => x.GroupId)
                .OrderByDescending(x => x.CreatedOn)
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakePost)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        private object MakePost(Post post)
        {
            return new
            {
                post.Id,
                post.Subject,
                post.Message,
                //Creator = _userViewFactory.Make(_documentSession.Load<User>(post.User.Id)), // TODO: Fix this n+1 prob
                //Comments = post.Discussion.Comments.Select(MakeComment), // TODO: Fix this n+1 prob
                Resources = post.MediaResources
            };
        }

        #endregion
    }
}