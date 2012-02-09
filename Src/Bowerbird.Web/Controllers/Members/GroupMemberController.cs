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
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class GroupMemberController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupMemberController(
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
        public ActionResult List(GroupMemberListInput listInput)
        {
            return Json(MakeGroupMemberList(listInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public ActionResult Create(GroupMemberCreateInput createInput)
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
        public ActionResult Delete(GroupMemberDeleteInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("success");
            }

            return Json("Failure");
        }

        private GroupMemberCreateCommand MakeCreateCommand(GroupMemberCreateInput createInput)
        {
            return new GroupMemberCreateCommand()
            {
                UserId = createInput.UserId,
                CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                GroupId = createInput.GroupId,
                Roles = createInput.Roles.ToList()
            };
        }

        private GroupMemberDeleteCommand MakeDeleteCommand(GroupMemberDeleteInput deleteInput)
        {
            return new GroupMemberDeleteCommand()
            {
                UserId = deleteInput.UserId,
                DeletedByUserId = _userContext.GetAuthenticatedUserId(),
                GroupId = deleteInput.GroupId
            };
        }

        private GroupMemberList MakeGroupMemberList(GroupMemberListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<GroupMember>()
                .Include<GroupMember>(x => x.Group.Id)
                .Where(x => x.Group.Id == listInput.GroupId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new GroupMemberList
            {
                Group = _documentSession.Load<Group>(listInput.GroupId),
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                GroupMembers = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}