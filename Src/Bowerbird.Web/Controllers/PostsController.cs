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
using Bowerbird.Web.Config;
using Bowerbird.Web.Queries;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class PostsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IPostsQuery _postsQuery;

        #endregion

        #region Constructors

        public PostsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IPostsQuery postsQuery
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(postsQuery, "postsQuery");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _postsQuery = postsQuery;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(PostListInput listInput)
        {
            return new JsonNetResult(_postsQuery.MakePostList(listInput));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(PostCreateInput createInput)
        {
            if(!_userContext.HasGroupPermission<Post>(createInput.GroupId, PermissionNames.CreatePost))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
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

            return Json("success");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(PostUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission<Post>(PermissionNames.UpdatePost, updateInput.Id))
            {
                return HttpUnauthorized();
            }
            
            if (!ModelState.IsValid)
            {
                return Json("Failure");
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

            return Json("success");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if(!_userContext.HasGroupPermission<Post>(PermissionNames.DeletePost, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(
                new PostDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = deleteInput.Id
                });

            return Json("success");
        }

        #endregion
    }
}