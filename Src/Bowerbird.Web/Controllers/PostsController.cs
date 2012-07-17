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
using System.Dynamic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class PostsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly IPermissionChecker _permissionChecker;

        #endregion

        #region Constructors

        public PostsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IPostViewModelBuilder postViewModelBuilder,
            IPermissionChecker permissionChecker
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(permissionChecker, "permissionChecker");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _postViewModelBuilder = postViewModelBuilder;
            _permissionChecker = permissionChecker;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string postId = VerbosifyId<Post>(id);

            if (!_permissionChecker.DoesExist<Post>(postId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Post = _postViewModelBuilder.BuildPost(id)
            };

            return RestfulResult(
                viewModel,
                "posts",
                "index");
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string id)
        {
            //if(!_permissionChecker.DoesExist<>())

            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Post = _postViewModelBuilder.BuildNewPost(id);

            return RestfulResult(
                viewModel,
                "posts",
                "create",
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string postId = VerbosifyId<Post>(id);

            if (!_permissionChecker.DoesExist<Post>(postId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdatePost))
            {
                return HttpUnauthorized();
            }

            var post = _postViewModelBuilder.BuildPost(postId);

            dynamic viewModel = new ExpandoObject();
            
            viewModel.Post = post;

            return RestfulResult(
                viewModel,
                "posts",
                "update",
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string postId = VerbosifyId<Post>(id);

            if (!_permissionChecker.DoesExist<Post>(postId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeletePost))
            {
                return HttpUnauthorized();
            }

            var post = _postViewModelBuilder.BuildPost(postId);

            dynamic viewModel = new ExpandoObject();
            
            viewModel.Post = post;

            return RestfulResult(
                viewModel,
                "posts",
                "delete", 
                new Action<dynamic>(x => x.Model.Delete = true));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(PostCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            if (!_userContext.HasGroupPermission(PermissionNames.CreatePost, createInput.GroupId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new PostCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = createInput.GroupId,
                    MediaResources = createInput.MediaResources,
                    Message = createInput.Message,
                    Subject = createInput.Subject,
                    Timestamp = createInput.Timestamp
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(PostUpdateInput updateInput)
        {
            Check.RequireNotNull(updateInput, "updateInput");

            if (!_permissionChecker.DoesExist<Post>(updateInput.Id))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Post>(PermissionNames.UpdatePost, updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new PostUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = updateInput.Id,
                    MediaResources = updateInput.MediaResources,
                    Message = updateInput.Message,
                    Subject = updateInput.Subject,
                    Timestamp = updateInput.Timestamp
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(string id)
        {
            var postId = VerbosifyId<Post>(id);

            if(!_permissionChecker.DoesExist<Post>(postId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Post>(PermissionNames.DeletePost, postId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new PostDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = postId 
                });

            return JsonSuccess();
        }

        #endregion
    }
}