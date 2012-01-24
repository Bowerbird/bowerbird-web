/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
    public class ProjectController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectController(
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
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(MakeProjectIndex(idInput));
            }

            return View(MakeProjectIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ProjectListInput listInput)
        {
            return Json(MakeProjectList(listInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeProjectCreateCommand(createInput));

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(ProjectUpdateInput updateInput) 
        {
            if (!_userContext.HasPermissionToUpdate<Project>(updateInput.ProjectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }
            
            _commandProcessor.Process(MakeProjectUpdateCommand(updateInput));

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasPermissionToDelete<Project>(deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeProjectDeleteCommand(deleteInput));

            return Json("Success");
        }

        private ProjectIndex MakeProjectIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession.Load<Project>(idInput.Id);

            var projectObservations =
                _documentSession
                .Query<ProjectObservation>()
                .Customize(x => x.Include(idInput.Id))
                .Where(x => x.Project.Id == idInput.Id)
                .ToList();

            var observations =
                _documentSession
                .Load<Observation>(projectObservations.Select(x => x.Observation.Id))
                .ToList();

            return new ProjectIndex()
            {
                Project = project,
                Observations = observations
            };
        }

        private ProjectList MakeProjectList(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Project>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ProjectList
            {
                TeamId = listInput.TeamId,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Projects = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private ProjectCreateCommand MakeProjectCreateCommand(ProjectCreateInput createInput)
        {
            return new ProjectCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectDeleteCommand MakeProjectDeleteCommand(IdInput deleteInput)
        {
            return new ProjectDeleteCommand()
            {
                Id = deleteInput.Id,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectUpdateCommand MakeProjectUpdateCommand(ProjectUpdateInput updateInput)
        {
            return new ProjectUpdateCommand()
            {
                Description = updateInput.Description,
                Name = updateInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        #endregion
    }
}