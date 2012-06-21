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
    public class PostsViewModelBuilder : IPostsViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public PostsViewModelBuilder(
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

        public object BuildPost(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return MakePost(_documentSession.Load<Post>(idInput.Id));
        }

        public object BuildPost(string groupId)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");

            return new
            {
                Subject = "New Subject",
                Message = "New Message",
                GroupId = groupId
            };
        }

        /// <summary>
        /// PagingInput.Id is User.Id
        /// </summary>
        public object BuildUserPostList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<Post>()
                .Where(x => x.User.Id == pagingInput.Id)
                .Include(x => x.GroupId)
                .OrderByDescending(x => x.CreatedOn)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakePost)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is Group.Id
        /// </summary>
        public object BuildGroupPostList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<Post>()
                .Where(x => x.GroupId == pagingInput.Id)
                .Include(x => x.GroupId)
                .OrderByDescending(x => x.CreatedOn)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakePost)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is Group.Id
        /// </summary>
        public object BuildPostStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.GroupId)
                .Where(x => x.ContributionType.Equals("post") && x.GroupId == pagingInput.Id)
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(
                    pagingInput.Page, 
                    pagingInput.PageSize, 
                    stats.TotalResults,
                    null);
        }

        private object MakeStreamItem(All_Contributions.Result groupContributionResult)
        {
            throw new NotImplementedException();
            //object item = null;
            //string description = null;
            //Group group = null;

            //switch (groupContributionResult.ContributionType)
            //{
            //    case "Post":
            //        item = MakePost(groupContributionResult.Post);
            //        description = groupContributionResult.Post.User.FirstName + " added a post";
            //        string postedToGroupType = null;
            //        group = groupContributionResult.GroupId
            //        break;
            //}

            //return MakeStreamItem(
            //    item,
            //    group,
            //    "post",
            //    groupContributionResult.GroupUser,
            //    groupContributionResult.GroupCreatedDateTime,
            //    description);
        }

        private object MakePost(Post post)
        {
            return new
            {
                post.Id,
                post.Subject,
                post.Message,
                Creator = MakeUser(post.User.Id),
                Comments = post.Discussion.Comments.Select(MakeComment),
                Resources = post.MediaResources
            };
        }

        private object MakeUser(string userId)
        {
            // HACK: Massive N+1 problem right here
            return _userViewFactory.Make(_documentSession.Load<User>(userId));
        }

        private object MakeComment(Comment comment)
        {
            return new
            {
                comment.Id,
                comment.CommentedOn,
                comment.Message,
                Creator = MakeUser(comment.User.Id)
            };
        }

        //private static object MakeStreamItem(
        //    object item,
        //    Group group,
        //    string contributionType,
        //    User groupUser,
        //    DateTime groupCreatedDateTime,
        //    string description
        //)
        //{
        //    return new
        //    {
        //        CreatedDateTime = groupCreatedDateTime,
        //        CreatedDateTimeDescription = groupCreatedDateTime.Description(),
        //        Type = contributionType.ToLower(),
        //        User = new
        //        {
        //            groupUser.Id,
        //            groupUser.LastLoggedIn,
        //            Name = groupUser.FirstName + " " + groupUser.LastName,
        //            Avatar = new
        //            {
        //                AltTag = groupUser.FirstName + " " + groupUser.LastName,
        //                UrlToImage = groupUser.Avatar != null ? "" : AvatarUris.DefaultUser
        //            }
        //        },
        //        Item = item,
        //        Description = description,
        //        Group = group
        //    };
        //}

        #endregion
    }
}