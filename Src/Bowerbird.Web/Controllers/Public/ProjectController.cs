///* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
//using Bowerbird.Core.Indexes;
//using Bowerbird.Core.Paging;
//using Bowerbird.Core.Services;
//using Bowerbird.Web.ViewModels.Shared;
//using Raven.Client;
//using Raven.Client.Linq;

//namespace Bowerbird.Web.Controllers.Public
//{
//    public class ProjectController : ControllerBase
//    {
//        #region Members

//        private readonly IDocumentSession _documentSession;
//        private readonly IMediaFilePathService _mediaFilePathService;
//        private readonly IConfigService _configService;

//        #endregion

//        #region Constructors

//        public ProjectController(
//            IDocumentSession documentSession,
//            IMediaFilePathService mediaFilePathService,
//            IConfigService configService)
//        {
//            Check.RequireNotNull(documentSession, "documentSession");
//            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
//            Check.RequireNotNull(configService, "configService");

//            _documentSession = documentSession;
//            _mediaFilePathService = mediaFilePathService;
//            _configService = configService;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        [HttpGet]
//        public ActionResult Index(IdInput idInput)
//        {
//            if (Request.IsAjaxRequest())

//                return Json(MakeProjectIndex(idInput));

//            return View(MakeProjectIndex(idInput));
//        }

//        [HttpGet]
//        public ActionResult List(ProjectListInput listInput)
//        {
//            if (listInput.TeamId != null)
//            {
//                return Json(MakeProjectListByTeamId(listInput), JsonRequestBehavior.AllowGet);
//            }

//            if (listInput.UserId != null)
//            {
//                return Json(MakeProjectListByMembership(listInput), JsonRequestBehavior.AllowGet);
//            }

//            return Json(MakeProjectList(listInput), JsonRequestBehavior.AllowGet);
//        }

//        private ProjectIndex MakeProjectIndex(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            var project = _documentSession.Load<Project>(idInput.Id);

//            return new ProjectIndex()
//            {
//                Project = project,
//                Avatar = GetAvatar(project),
//                Team = project.ParentGroupId != null ? _documentSession.Load<Team>(project.ParentGroupId) : null
//            };
//        }

//        private ProjectList MakeProjectList(ProjectListInput listInput)
//        {
//            RavenQueryStatistics stats;

//            var results = _documentSession
//                .Query<Project>()
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList()
//                .Select(x => new ProjectView()
//                {
//                    Id = x.Id,
//                    Description = x.Description,
//                    Name = x.Name,
//                    Website = x.Website,
//                    Avatar = GetAvatar(x)
//                });

//            return new ProjectList
//            {
//                Page = listInput.Page,
//                PageSize = listInput.PageSize,
//                Projects = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        private ProjectList MakeProjectListByTeamId(ProjectListInput listInput)
//        {
//            RavenQueryStatistics stats;

//            var results = _documentSession
//                .Query<Project>()
//                .Where(x => x.ParentGroupId == listInput.TeamId)
//                .Customize(x => x.Include<Team>(y => y.Id == listInput.TeamId))
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList()
//                .Select(x => new ProjectView()
//                {
//                    Id = x.Id,
//                    Description = x.Description,
//                    Name = x.Name,
//                    Website = x.Website,
//                    Avatar = GetAvatar(x)
//                });

//            return new ProjectList
//            {
//                Team = listInput.TeamId != null ? _documentSession.Load<Team>(listInput.TeamId) : null,
//                Page = listInput.Page,
//                PageSize = listInput.PageSize,
//                Projects = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        private ProjectList MakeProjectListByMembership(ProjectListInput listInput)
//        {
//            RavenQueryStatistics stats;

//            var groupMemberships = _documentSession
//                .Query<GroupMember, All_Members>()
//                .Where(x => x.User.Id == listInput.UserId);

//            var results = _documentSession
//                .Query<Project>()
//                .Where(x => x.Id.In(groupMemberships.Select(y => y.Group.Id)))
//                .Customize(x => x.Include<User>(y => y.Id == listInput.UserId))
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList()
//                .Select(x => new ProjectView()
//                {
//                    Id = x.Id,
//                    Description = x.Description,
//                    Name = x.Name,
//                    Website = x.Website,
//                    Avatar = GetAvatar(x)
//                });

//            return new ProjectList
//            {
//                User = listInput.UserId != null ? _documentSession.Load<User>(listInput.UserId) : null,
//                Page = listInput.Page,
//                PageSize = listInput.PageSize,
//                Projects = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        private Avatar GetAvatar(Project project)
//        {
//            return new Avatar()
//            {
//                AltTag = project.Description,
//                UrlToImage = project.Avatar != null ?
//                    _mediaFilePathService.MakeMediaFileUri(project.Avatar.Id, "image", "avatar", project.Avatar.Metadata["metatype"]) :
//                    _configService.GetDefaultAvatar("project")
//            };
//        }

//        #endregion
//    }
//}