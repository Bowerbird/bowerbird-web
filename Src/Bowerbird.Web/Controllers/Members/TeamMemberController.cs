/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Team Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class TeamMemberController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamMemberController(
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
        public ActionResult List(TeamMemberListInput listInput)
        {
            return Json(MakeTeamMemberList(listInput));
        }

        [Transaction]
        [HttpPost]
        public ActionResult Create(TeamMemberCreateInput createInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeCreateCommand(createInput));

                return Json("success");
            }

            return Json("Failure");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(TeamMemberDeleteInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("success");
            }

            return Json("Failure");
        }

        private TeamMemberCreateCommand MakeCreateCommand(TeamMemberCreateInput createInput)
        {
            return new TeamMemberCreateCommand()
            {
                UserId = createInput.UserId,
                CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                TeamId = createInput.TeamId,
                Roles = createInput.Roles.ToList()
            };
        }

        private TeamMemberDeleteCommand MakeDeleteCommand(TeamMemberDeleteInput deleteInput)
        {
            return new TeamMemberDeleteCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                DeletedByUserId = _userContext.GetAuthenticatedUserId(),
                TeamId = deleteInput.TeamId
            };
        }

        private TeamMemberList MakeTeamMemberList(TeamMemberListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<TeamMember>()
                .Where(x => x.Team.Id == listInput.TeamId)
                .Customize(x => x.Include(listInput.TeamId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new TeamMemberList
            {
                Team = _documentSession.Load<Team>(listInput.TeamId),
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                TeamMembers = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}