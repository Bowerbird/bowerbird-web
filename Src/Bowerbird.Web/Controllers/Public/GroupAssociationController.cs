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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Public
{
    public class GroupAssociationController : ControllerBase
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupAssociationController(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(GroupAssociationChildListInput listInput)
        {
            return Json(MakeGroupAssociationChildList(listInput), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult List(GroupAssociationParentListInput listInput)
        {
            return Json(MakeGroupAssociationParentList(listInput), JsonRequestBehavior.AllowGet);
        }

        private GroupAssociationList MakeGroupAssociationParentList(GroupAssociationParentListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ChildGroupId == listInput.ChildGroupId)
                .Include(x => x.ParentGroupId)
                .Include(x => x.ChildGroupId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new GroupAssociationList
            {
                Group = _documentSession.Load<Group>(listInput.ChildGroupId),
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                GroupAssociations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private GroupAssociationList MakeGroupAssociationChildList(GroupAssociationChildListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == listInput.ParentGroupId)
                .Include(x => x.ParentGroupId)
                .Include(x => x.ChildGroupId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new GroupAssociationList
            {
                Group = _documentSession.Load<Group>(listInput.ParentGroupId),
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                GroupAssociations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}