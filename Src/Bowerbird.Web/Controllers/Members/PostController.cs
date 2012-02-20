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
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class PostController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PostController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(PostListInput listInput)
        {
            return Json(MakePostList(listInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public ActionResult Create(PostCreateInput createInput)
        {
            if(!_userContext.HasGroupPermission(createInput.GroupId, Permissions.CreatePost))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(PostUpdateInput updateInput)
        {
            if (!_userContext.HasPermissionToUpdate<Post>(updateInput.Id))
            {
                return HttpUnauthorized();
            }
            
            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeUpdateCommand(updateInput));

            return Json("success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if(!_userContext.HasPermissionToDelete<Post>(deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        private PostList MakePostList(PostListInput listInput)
        {
            RavenQueryStatistics stats;

            var posts = _documentSession
                .Query<Post>()
                .Where(x => x.GroupContributions.Any(y => y.GroupId == listInput.GroupId))
                .OrderByDescending(x => x.CreatedOn)
                .Customize(x => x.Include(listInput.GroupId))
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

        private PostCreateCommand MakeCreateCommand(PostCreateInput createInput)
        {
            return new PostCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                GroupId = createInput.GroupId,
                MediaResources = createInput.MediaResources,
                Message = createInput.Message,
                Subject = createInput.Subject,
                Timestamp = createInput.Timestamp
            };
        }

        private PostDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new PostDeleteCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Id = deleteInput.Id
            };
        }

        private PostUpdateCommand MakeUpdateCommand(PostUpdateInput updateInput)
        {
            return new PostUpdateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Id = updateInput.Id,
                MediaResources = updateInput.MediaResources,
                Message = updateInput.Message,
                Subject = updateInput.Subject,
                Timestamp = updateInput.Timestamp
            };
        }

        #endregion
    }
}