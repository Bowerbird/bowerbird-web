///* Bowerbird V1 

// Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Linq;
//using System.Web.Mvc;
//using Bowerbird.Core.Commands;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.Paging;
//using Bowerbird.Web.Config;
//using Bowerbird.Web.ViewModels;
//using Raven.Client;
//using Raven.Client.Linq;

//namespace Bowerbird.Web.Controllers.Members
//{
//    public class MemberController : ControllerBase
//    {
//        #region Members

//        private readonly ICommandProcessor _commandProcessor;
//        private readonly IUserContext _userContext;
//        private readonly IDocumentSession _documentSession;

//        #endregion

//        #region Constructors

//        public MemberController(
//            ICommandProcessor commandProcessor,
//            IUserContext userContext,
//            IDocumentSession documentSession)
//        {
//            Check.RequireNotNull(commandProcessor, "commandProcessor");
//            Check.RequireNotNull(userContext, "userContext");
//            Check.RequireNotNull(documentSession, "documentSession");

//            _commandProcessor = commandProcessor;
//            _userContext = userContext;
//            _documentSession = documentSession;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        [HttpGet]
//        public ActionResult List(MemberListInput listInput)
//        {
//            return Json(MakeMemberList(listInput), JsonRequestBehavior.AllowGet);
//        }

//        [Transaction]
//        [HttpPost]
//        public ActionResult Create(MemberCreateInput createInput)
//        {
//            if (ModelState.IsValid)
//            {
//                _commandProcessor.Process(MakeCreateCommand(createInput));

//                return Json("success");
//            }

//            return Json("Failure");
//        }

//        [Transaction]
//        [HttpDelete]
//        public ActionResult Delete(MemberDeleteInput deleteInput)
//        {
//            if (ModelState.IsValid)
//            {
//                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

//                return Json("success");
//            }

//            return Json("Failure");
//        }

//        private MemberCreateCommand MakeCreateCommand(MemberCreateInput createInput)
//        {
//            return new MemberCreateCommand()
//            {
//                UserId = createInput.UserId,
//                CreatedByUserId = _userContext.GetAuthenticatedUserId(),
//                GroupId = createInput.GroupId,
//                Roles = createInput.Roles.ToList()
//            };
//        }

//        private MemberDeleteCommand MakeDeleteCommand(MemberDeleteInput deleteInput)
//        {
//            return new MemberDeleteCommand()
//            {
//                UserId = deleteInput.UserId,
//                DeletedByUserId = _userContext.GetAuthenticatedUserId(),
//                GroupId = deleteInput.GroupId
//            };
//        }

//        private MemberList MakeMemberList(MemberListInput listInput)
//        {
//            RavenQueryStatistics stats;

//            var results = _documentSession
//                .Query<GroupMember>()
//                .Include<GroupMember>(x => x.Group.Id)
//                .Where(x => x.Group.Id == listInput.GroupId)
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

//            return new MemberList
//            {
//                Group = _documentSession.Load<Group>(listInput.GroupId),
//                Page = listInput.Page,
//                PageSize = listInput.PageSize,
//                Members = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        #endregion
//    }
//}