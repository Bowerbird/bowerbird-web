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
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Public
{
    public class ProjectController : ControllerBase
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectController(
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
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())

                return Json(MakeProjectIndex(idInput));

            return View(MakeProjectIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ProjectListInput listInput)
        {
            if (listInput.TeamId != null)
            {
                return Json(MakeProjectListByTeamId(listInput), JsonRequestBehavior.AllowGet);
            }

            if (listInput.UserId != null)
            {
                return Json(MakeProjectListByMembership(listInput), JsonRequestBehavior.AllowGet);
            }

            return Json(MakeProjectList(listInput), JsonRequestBehavior.AllowGet);
        }

        protected ProjectIndex MakeProjectIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession.Load<Project>(idInput.Id);

            return new ProjectIndex()
            {
                Project = project,

                Team = project.ParentGroupId != null ? _documentSession.Load<Team>(project.ParentGroupId) : null
            };
        }

        protected ProjectList MakeProjectList(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Project>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            return new ProjectList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Projects = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        protected ProjectList MakeProjectListByTeamId(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Project>()
                .Where(x => x.ParentGroupId == listInput.TeamId)
                .Customize(x => x.Include<Team>(y => y.Id == listInput.TeamId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            return new ProjectList
            {
                Team = listInput.TeamId != null ? _documentSession.Load<Team>(listInput.TeamId) : null,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Projects = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        protected ProjectList MakeProjectListByMembership(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var groupMemberships = _documentSession
                .Query<GroupMember, All_Members>()
                .Where(x => x.User.Id == listInput.UserId);

            var results = _documentSession
                .Query<Project>()
                .Where(x => x.Id.In(groupMemberships.Select(y => y.Group.Id)))
                .Customize(x => x.Include<User>(y => y.Id == listInput.UserId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            return new ProjectList
            {
                User = listInput.UserId != null ? _documentSession.Load<User>(listInput.UserId) : null,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Projects = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}