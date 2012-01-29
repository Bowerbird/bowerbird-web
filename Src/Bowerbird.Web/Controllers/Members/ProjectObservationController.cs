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
using Bowerbird.Web.Validators;
using Bowerbird.Web.ViewModels.Members;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class ProjectObservationController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectObservationController(
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
        public ActionResult List(ProjectObservationListInput listInput)
        {
            return Json(MakeProjectList(listInput));
        }

        [Transaction]
        [HandleModelStateException]
        [HttpPost]
        public ActionResult Create(ProjectObservationCreateInput createInput)
        {
            if (!_userContext.HasProjectPermission(createInput.ProjectId, Permissions.CreateProjectObservation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                //throw new ModelStateException(ModelState);
                return Json("failure");
            }

            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ProjectObservationDeleteInput deleteInput)
        {
            if(!_userContext.HasProjectObservationDeletePermission(deleteInput.ObservationId, deleteInput.ProjectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                //throw new ModelStateException(ModelState);
                return Json("failure");
            }

            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        private ProjectObservationList MakeProjectList(ProjectObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<ProjectObservation>()
                .Where(x => x.Project.Id == listInput.ProjectId)
                .Customize(x => x.Include(listInput.ProjectId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ProjectObservationList
            {
                Project = _documentSession.Load<Project>(listInput.ProjectId),
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                ProjectObservations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private ProjectObservationCreateCommand MakeCreateCommand(ProjectObservationCreateInput createInput)
        {
            return new ProjectObservationCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                ObservationId = createInput.ObservationId,
                ProjectId = createInput.ProjectId
            };
        }

        private ProjectObservationDeleteCommand MakeDeleteCommand(ProjectObservationDeleteInput deleteInput)
        {
            return new ProjectObservationDeleteCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                ProjectId = deleteInput.ProjectId,
                ObservationId = deleteInput.ObservationId
            };
        }

        #endregion
    }
}