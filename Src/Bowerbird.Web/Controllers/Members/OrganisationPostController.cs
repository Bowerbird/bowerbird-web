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
    public class OrganisationPostController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationPostController(
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
        public ActionResult List(OrganisationPostListInput listInput)
        {
            return Json(MakeOrganisationPostList(listInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public ActionResult Create(OrganisationPostCreateInput createInput)
        {
            if (!_userContext.HasOrganisationPermission(createInput.OrganisationId, Permissions.CreateOrganisationPost))
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
        public ActionResult Update(OrganisationPostUpdateInput updateInput)
        {
            if (!_userContext.HasPermissionToUpdate<OrganisationPost>(updateInput.Id))
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
            if (!_userContext.HasPermissionToDelete<OrganisationPost>(deleteInput.Id))
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

        private OrganisationPostList MakeOrganisationPostList(OrganisationPostListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<OrganisationPost>()
                .Where(x => x.Organisation.Id == listInput.OrganisationId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new OrganisationPostList
            {
                OrganisationId = listInput.OrganisationId,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Posts = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private OrganisationPostCreateCommand MakeCreateCommand(OrganisationPostCreateInput createInput)
        {
            return new OrganisationPostCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                OrganisationId = createInput.OrganisationId,
                MediaResources = createInput.MediaResources.ToList(),
                Message = createInput.Message,
                Subject = createInput.Subject,
                PostedOn = createInput.Timestamp
            };
        }

        private OrganisationPostDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new OrganisationPostDeleteCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Id = deleteInput.Id
            };
        }

        private OrganisationPostUpdateCommand MakeUpdateCommand(OrganisationPostUpdateInput updateInput)
        {
            return new OrganisationPostUpdateCommand()
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