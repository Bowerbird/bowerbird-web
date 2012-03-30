///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.Extensions;
//using Bowerbird.Core.Indexes;
//using Bowerbird.Core.Paging;
//using Bowerbird.Core.Services;
//using Bowerbird.Web.ViewModels.Members;
//using Bowerbird.Web.ViewModels.Shared;
//using Raven.Client;
//using Raven.Client.Linq;

//namespace Bowerbird.Web.Controllers.Public  
//{
//    public class TeamController : ControllerBase
//    {
//        #region Members

//        private readonly IDocumentSession _documentSession;
//        private readonly IMediaFilePathService _mediaFilePathService;
//        private readonly IConfigService _configService;

//        #endregion

//        #region Constructors

//        public TeamController(
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
//        public ActionResult List(TeamListInput listInput)
//        {
//            if (listInput.OrganisationId != null)
//            {
//                return Json(MakeTeamListByOrganisationId(listInput), JsonRequestBehavior.AllowGet);
//            }
//            if (listInput.UserId != null)
//            {
//                return Json(MakeTeamListByMembership(listInput), JsonRequestBehavior.AllowGet);
//            }

//            return Json(MakeTeamList(listInput), JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public ActionResult Index(IdInput idInput)
//        {
//            if (Request.IsAjaxRequest())

//                return Json(MakeTeamIndex(idInput));

//            return View(MakeTeamIndex(idInput));
//        }

//        private TeamIndex MakeTeamIndex(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            var team = _documentSession.Load<Team>(idInput.Id);

//            var organisation = team.ParentGroupId != null
//                                   ? _documentSession.Load<Organisation>(team.ParentGroupId)
//                                   : null;

//            var associatedGroups = _documentSession
//                .Query<GroupAssociation>()
//                .Where(x => x.GroupId == team.Id);

//            IEnumerable<Project> projects = null;

//            if(associatedGroups.IsNotNullAndHasItems())
//            {
//                projects = _documentSession
//                    .Query<Project>()
//                    .Where(x => associatedGroups.All(y => y.GroupId == x.Id));
//            }

//            return new TeamIndex()
//            {
//                Team = team,
//                Organisation = organisation,
//                Projects = projects,
//                Avatar = GetAvatar(team)
//            };
//        }

//        private TeamList MakeTeamList(TeamListInput listInput)
//        {
//            RavenQueryStatistics stats;

//            var results = _documentSession
//                .Query<Team>()
//                .Customize(x => x.Include<Organisation>(y => y.ParentGroupId == listInput.OrganisationId))
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList() // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
//                .Select(x => new TeamView()
//                {
//                    Id = x.Id,
//                    Description = x.Description,
//                    Name = x.Name,
//                    Website = x.Website,
//                    Avatar = GetAvatar(x)
//                });

//            return new TeamList()
//            {
//                Organisation = listInput.OrganisationId != null ? _documentSession.Load<Organisation>(listInput.OrganisationId) : null,
//                Page = listInput.Page,
//                PageSize = listInput.PageSize,
//                Teams = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        private TeamList MakeTeamListByOrganisationId(TeamListInput listInput)
//        {
//            RavenQueryStatistics stats;

//            var results = _documentSession
//                .Query<Team>()
//                .Where(x => x.ParentGroupId == listInput.OrganisationId)
//                .Customize(x => x.Include<Organisation>(y => y.Id == listInput.OrganisationId))
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList() // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
//                .Select(x => new TeamView()
//                {
//                    Id = x.Id,
//                    Description = x.Description,
//                    Name = x.Name,
//                    Website = x.Website,
//                    Avatar = GetAvatar(x)
//                });

//            return new TeamList()
//            {
//                Organisation = listInput.OrganisationId != null ? _documentSession.Load<Organisation>(listInput.OrganisationId) : null,
//                Page = listInput.Page,
//                PageSize = listInput.PageSize,
//                Teams = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        private TeamList MakeTeamListByMembership(TeamListInput listInput)
//        {
//            RavenQueryStatistics stats;

//            var groupMemberships = _documentSession
//                .Query<GroupMember, All_Members>()
//                .Where(x => x.User.Id == listInput.UserId);

//            var results = _documentSession
//                .Query<Team>()
//                .Where(x => x.Id.In(groupMemberships.Select(y => y.Group.Id)))
//                .Customize(x => x.Include<User>(y => y.Id == listInput.UserId))
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList()
//                .Select(x => new TeamView()
//                {
//                    Id = x.Id,
//                    Description = x.Description,
//                    Name = x.Name,
//                    Website = x.Website,
//                    Avatar = GetAvatar(x)
//                });

//            return new TeamList
//            {
//                User = listInput.UserId != null ? _documentSession.Load<User>(listInput.UserId) : null,
//                Page = listInput.Page,
//                PageSize = listInput.PageSize,
//                Teams = results.ToPagedList(
//                    listInput.Page,
//                    listInput.PageSize,
//                    stats.TotalResults,
//                    null)
//            };
//        }

//        private Avatar GetAvatar(Team team)
//        {
//            return new Avatar()
//            {
//                AltTag = team.Description,
//                UrlToImage = team.Avatar != null ?
//                    _mediaFilePathService.MakeMediaFileUri(team.Avatar.Id, "image", "avatar", team.Avatar.Metadata["metatype"]) :
//                    _configService.GetDefaultAvatar("team")
//            };
//        }

//        #endregion
//    }
//}