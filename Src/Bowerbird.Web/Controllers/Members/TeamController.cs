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
    public class TeamController : ControllerBase
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
        public ActionResult List(TeamListInput listInput)
        {
            return Json(MakeTeamList(listInput), JsonRequestBehavior.AllowGet);
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
        public ActionResult Create(TeamCreateInput createInput)
        {
            if (!_userContext.HasGlobalPermission(Permissions.CreateTeam))
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
        public ActionResult Update(TeamUpdateInput updateInput)
        {
            if (!_userContext.HasPermissionToUpdate<Team>(updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeUpdateCommand(updateInput));

            return Json("Success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasPermissionToDelete<Team>(deleteInput.Id))
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

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult CreateProject(ProjectCreateInput projectCreateInput, TeamProjectCreateInput teamProjectCreateInput)
        {
            if(_userContext.HasTeamPermission(teamProjectCreateInput.TeamId, Permissions.CreateTeamProject))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeTeamProjectCreateCommand(projectCreateInput, teamProjectCreateInput));

            return Json("success");
        }

        private TeamIndex MakeIndex(IdInput idInput)
        {
            var team = _documentSession.Load<Team>(idInput.Id);

            var projects =
                Queryable.Where(_documentSession
                           .Query<Project>(), x => x.Team.Id == idInput.Id)
                .ToList();

            return new TeamIndex()
            {
                Team = team,
                Projects = projects
            };
        }

        private TeamList MakeTeamList(TeamListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Team>()
                .Where(x => x.Organisation.Id == listInput.OrganisationId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new TeamList()
            {
                OrganisationId = listInput.OrganisationId,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Teams = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
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