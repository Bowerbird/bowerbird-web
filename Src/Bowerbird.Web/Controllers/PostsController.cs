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
using System.Web.Mvc;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Queries;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using System;
using Bowerbird.Core.Config;
using Bowerbird.Web.Infrastructure;
using Raven.Client;
using System.Collections;
using System.Dynamic;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class PostsController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IPostViewModelQuery _postViewModelQuery;
        private readonly IDocumentSession _documentSession;
        private readonly IPermissionManager _permissionManager;

        #endregion

        #region Constructors

        public PostsController(
            IMessageBus messageBus,
            IUserContext userContext,
            IPostViewModelQuery postViewModelQuery,
            IDocumentSession documentSession,
            IPermissionManager permissionManager
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(postViewModelQuery, "postViewModelQuery");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionManager, "permissionManager");

            _messageBus = messageBus;
            _userContext = userContext;
            _postViewModelQuery = postViewModelQuery;
            _documentSession = documentSession;
            _permissionManager = permissionManager;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string postId = VerbosifyId<Post>(id);

            if (!_permissionManager.DoesExist<Post>(postId))
            {
                return HttpNotFound();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Post = _postViewModelQuery.BuildPost(postId);

            return RestfulResult(
                viewModel,
                "posts",
                "index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string groupId, string groupType)
        {
            var actualGroupId = groupType + "/" + groupId;

            if (!_userContext.HasGroupPermission(PermissionNames.CreatePost, actualGroupId))
            {
                return new HttpUnauthorizedResult();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Post = _postViewModelQuery.BuildCreatePost(actualGroupId);
            viewModel.PostTypeSelectList = GetPostTypeSelectList(string.Empty);

            return RestfulResult(
                viewModel,
                "posts",
                "create");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string postId = VerbosifyId<Post>(id);

            //if (!_permissionManager.DoesExist<Post>(postId))
            //{
            //    return HttpNotFound();
            //}

            //if (!_userContext.HasUserProjectPermission(PermissionNames.UpdatePost))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var post = _documentSession.Load<Post>(postId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Post = _postViewModelQuery.BuildPost(postId);
            viewModel.PostTypeSelectList = GetPostTypeSelectList(post.PostType);

            return RestfulResult(
                viewModel,
                "posts",
                "update");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string postId = VerbosifyId<Post>(id);

            if (!_permissionManager.DoesExist<Post>(postId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeletePost))
            {
                return new HttpUnauthorizedResult();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Post = _postViewModelQuery.BuildPost(postId);

            return RestfulResult(
                viewModel,
                "posts",
                "delete");
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(PostUpdateInput createInput)
        {
            var actualGroupId = createInput.GroupType + "/" + createInput.GroupId;

            //if (!_userContext.HasUserProjectPermission(PermissionNames.CreatePost))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            if (ModelState.IsValid)
            {
                var key = string.IsNullOrWhiteSpace(createInput.Key) ? Guid.NewGuid().ToString() : createInput.Key;

                _messageBus.Send(
                    new PostCreateCommand()
                        {
                            Key = key,
                            Subject = createInput.Subject,
                            Message = createInput.Message,
                            PostType = createInput.PostType,
                            UserId = _userContext.GetAuthenticatedUserId(),
                            GroupId = actualGroupId,
                            MediaResources = createInput.MediaResources
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Post = createInput;

            return RestfulResult(
                viewModel,
                "posts",
                "create");
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(PostUpdateInput updateInput)
        {
            string postId = VerbosifyId<Post>(updateInput.Id);

            //if (!_permissionManager.DoesExist<Post>(postId))
            //{
            //    return HttpNotFound();
            //}

            //if (!_userContext.HasGroupPermission<Post>(PermissionNames.UpdatePost, postId))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new PostUpdateCommand
                        {
                            Id = postId,
                            Subject = updateInput.Subject,
                            Message = updateInput.Message,
                            PostType = updateInput.PostType,
                            UserId = _userContext.GetAuthenticatedUserId(),
                            MediaResources = updateInput.MediaResources
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Post = updateInput;

            return RestfulResult(
                viewModel,
                "posts",
                "update");
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string postId = VerbosifyId<Post>(id);

            if (!_permissionManager.DoesExist<Post>(postId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Post>(PermissionNames.UpdatePost, postId))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new PostDeleteCommand
                {
                    Id = id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private object GetPostTypeSelectList(string selected)
        {
            return new[]
                {
                    new
                        {
                            Text = "General news",
                            Value = "news",
                            Selected = selected == "news"
                        },
                    new
                        {
                            Text = "Meeting",
                            Value = "meeting",
                            Selected = selected == "meeting"
                        },
                    new
                        {
                            Text = "Newsletter",
                            Value = "newsletter",
                            Selected = selected == "newsletter"
                        }
                };
        }

        #endregion
    }
}