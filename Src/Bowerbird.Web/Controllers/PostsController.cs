/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

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
    [Restful]
    public class PostsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IPostsViewModelBuilder _postsViewModelBuilder;

        #endregion

        #region Constructors

        public PostsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IPostsViewModelBuilder postsViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(postsViewModelBuilder, "postsViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _postsViewModelBuilder = postsViewModelBuilder;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            Check.RequireNotNull(idInput,"idInput");

            ViewBag.Post = _postsViewModelBuilder.BuildPost(idInput);

            return View(Form.Index);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return new JsonNetResult(_postsViewModelBuilder.BuildPost(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            return new JsonNetResult(_postsViewModelBuilder.BuildGroupPostList(pagingInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.CreatePost, idInput.Id))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new
            {
                Post = _postsViewModelBuilder.BuildPost(idInput.Id)
            };

            ViewBag.PrerenderedView = "post";

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateSpecies))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new
            {
                Post = _postsViewModelBuilder.BuildPost(idInput)
            };

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteSpecies))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new
            {
                Post = _postsViewModelBuilder.BuildPost(idInput)
            };

            return View(Form.Delete);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(PostCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            if(!_userContext.HasGroupPermission<Post>(createInput.GroupId, PermissionNames.CreatePost))
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
        public ActionResult Delete(IdInput deleteInput)
        {
            Check.RequireNotNull(deleteInput, "deleteInput");

            if(!_userContext.HasGroupPermission<Post>(PermissionNames.DeletePost, deleteInput.Id))
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
                    Id = deleteInput.Id
                });

            return JsonSuccess();
        }

        #endregion
    }
}