/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
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
    public class GroupAssociationController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupAssociationController(
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
        public ActionResult List(GroupAssociationChildListInput listInput)
        {
            return Json(MakeGroupAssociationChildList(listInput), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult List(GroupAssociationParentListInput listInput)
        {
            return Json(MakeGroupAssociationParentList(listInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public ActionResult Create(GroupAssociationCreateInput createInput)
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
        public ActionResult Delete(GroupAssociationDeleteInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("success");
            }

            return Json("Failure");
        }

        private GroupAssociationCreateCommand MakeCreateCommand(GroupAssociationCreateInput createInput)
        {
            return new GroupAssociationCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                ParentGroupId = createInput.ParentGroupId,
                ChildGroupId = createInput.ChildGroupId,
                CreatedDateTime = DateTime.Now
            };
        }

        private GroupAssociationDeleteCommand MakeDeleteCommand(GroupAssociationDeleteInput deleteInput)
        {
            return new GroupAssociationDeleteCommand()
            {
                ParentGroupId = deleteInput.ParentGroupId,
                ChildGroupId = deleteInput.ChildGroupId
            };
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