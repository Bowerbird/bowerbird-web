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
using Bowerbird.Core;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Members
{
    public class TeamController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamController(
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
        public ActionResult List(int? id, int? page, int? pageSize)
        {
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())
            
                return Json(MakeIndex(idInput));

            return View(MakeIndex(idInput));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(TeamCreateInput teamCreateInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeCreateCommand(teamCreateInput));

                return Json("success");
            }
            return Json("Failure");
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(TeamUpdateInput updateInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUpdateCommand(updateInput));

                return Json("Success");
            }
            return Json("Failure");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("success");
            }
            return Json("Failure");
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult CreateProject(ProjectCreateInput projectCreateInput, TeamProjectCreateInput teamProjectCreateInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeTeamProjectCreateCommand(projectCreateInput, teamProjectCreateInput));

                return Json("success");
            }
            return Json("Failure");
        }

        private TeamIndex MakeIndex(IdInput idInput)
        {
            var team = _documentSession.Load<Team>(idInput.Id);

            var projects =
                _documentSession
                .Query<Project>()
                .Where(x => x.Team.Id == idInput.Id)
                .ToList();

            return new TeamIndex()
            {
                Team = team,
                Projects = projects
            };
        }

        private TeamCreateCommand MakeCreateCommand(TeamCreateInput createInput)
        {
            return new TeamCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private TeamDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new TeamDeleteCommand()
            {
                Id = deleteInput.Id,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private TeamUpdateCommand MakeUpdateCommand(TeamUpdateInput updateInput)
        {
            return new TeamUpdateCommand()
            {
                Description = updateInput.Description,
                Name = updateInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private TeamProjectCreateCommand MakeTeamProjectCreateCommand(ProjectCreateInput projectCreateInput, TeamProjectCreateInput teamProjectCreateInput)
        {
            return new TeamProjectCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Name = projectCreateInput.Name,
                Description = projectCreateInput.Description,
                Administrators = teamProjectCreateInput.Administrators,
                Members = teamProjectCreateInput.Members,
                TeamId = teamProjectCreateInput.TeamId
            };
        }

        #endregion
    }
}