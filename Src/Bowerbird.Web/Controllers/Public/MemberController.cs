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
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.Paging;
//using Bowerbird.Web.ViewModels.Members;
//using Raven.Client;
//using Raven.Client.Linq;

//namespace Bowerbird.Web.Controllers.Public
//{
//    public class GroupMemberController : ControllerBase
//    {
//        #region Members

//        private readonly IDocumentSession _documentSession;

//        #endregion

//        #region Constructors

//        public GroupMemberController(
//            IDocumentSession documentSession)
//        {
//            Check.RequireNotNull(documentSession, "documentSession");

//            _documentSession = documentSession;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        [HttpGet]
//        public ActionResult List(MemberListInput listInput)
//        {
//            return Json(MakeGroupMemberList(listInput), JsonRequestBehavior.AllowGet);
//        }

//        private MemberList MakeGroupMemberList(MemberListInput listInput)
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
//                GroupMembers = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        #endregion
//    }
//}