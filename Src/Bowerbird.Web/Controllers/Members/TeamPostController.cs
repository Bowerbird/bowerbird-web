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
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class TeamPostController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamPostController(
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
        public ActionResult List(TeamPostListInput listInput)
        {
            return Json(MakeTeamPostList(listInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public ActionResult Create(TeamPostCreateInput createInput)
        {
            if (!_userContext.HasTeamPermission(createInput.TeamId, Permissions.CreateTeamPost))
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
        public ActionResult Update(TeamPostUpdateInput updateInput)
        {
            if (!_userContext.HasPermissionToUpdate<TeamPost>(updateInput.Id))
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
            if (!_userContext.HasPermissionToDelete<TeamPost>(deleteInput.Id))
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

        private TeamPostList MakeTeamPostList(TeamPostListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<TeamPost>()
                .Where(x => x.Team.Id == listInput.TeamId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new TeamPostList
            {
                TeamId = listInput.TeamId,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Posts = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private TeamPostCreateCommand MakeCreateCommand(TeamPostCreateInput createInput)
        {
            return new TeamPostCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                TeamId = createInput.TeamId,
                MediaResources = createInput.MediaResources.ToList(),
                Message = createInput.Message,
                Subject = createInput.Subject,
                PostedOn = createInput.Timestamp
            };
        }

        private TeamPostDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new TeamPostDeleteCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Id = deleteInput.Id
            };
        }

        private TeamPostUpdateCommand MakeUpdateCommand(TeamPostUpdateInput updateInput)
        {
            return new TeamPostUpdateCommand()
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