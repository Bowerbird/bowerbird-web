/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Queries;
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Config;
using Nustache.Mvc;

namespace Bowerbird.Web.Controllers
{
    public class TeamController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;
        private readonly IUsersGroupsHavingPermissionQuery _usersGroupsHavingPermissionQuery;

        #endregion

        #region Constructors

        public TeamController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IUsersGroupsHavingPermissionQuery usersGroupsHavingPermissionQuery,
            IConfigService configService)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(usersGroupsHavingPermissionQuery, "usersGroupsHavingPermissionQuery");
            Check.RequireNotNull(configService, "configService");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
            _usersGroupsHavingPermissionQuery = usersGroupsHavingPermissionQuery;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(MakeTeamIndex(idInput));
                }

                return View(MakeTeamIndex(idInput));
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List(TeamListInput listInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (listInput.HasAddProjectPermission)
                {
                    return new JsonNetResult(GetGroupsHavingAddProjectPermission());
                }

                if (Request.IsAjaxRequest())
                {
                    return new JsonNetResult(MakeTeamList(listInput));
                }
            }

            return View();
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult Teams()
        {
            ViewData["Groups"] = MakeTeamList(new TeamListInput() { Page = 1, PageSize = 10 }).Teams.PagedListItems;
            var viewResult = View("groupList");
            viewResult.ViewEngineCollection = new ViewEngineCollection { new NustacheViewEngine() };

            return viewResult;
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(TeamCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateTeam, createInput.Organisation ?? Constants.AppRootId))
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
            if (!_userContext.HasGroupPermission<Team>(PermissionNames.UpdateTeam, updateInput.Id))
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
            if (!_userContext.HasGroupPermission<Team>(PermissionNames.DeleteTeam, deleteInput.Id))
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
            if(_userContext.HasGroupPermission(teamProjectCreateInput.TeamId, PermissionNames.CreateProject))
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

        private TeamCreateCommand MakeCreateCommand(TeamCreateInput createInput)
        {
            return new TeamCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId(),
                OrganisationId = createInput.Organisation
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
                UserId = _userContext.GetAuthenticatedUserId(),
                AvatarId = updateInput.AvatarId
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

        private TeamIndex MakeTeamIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var team = _documentSession.Load<Team>(idInput.Id);

            //var organisation = team.ParentGroupId != null
            //                       ? _documentSession.Load<Organisation>(team.ParentGroupId)
            //                       : null;

            var groupAssociations = _documentSession
                .Query<GroupAssociation>()
                .Include(x => x.ChildGroupId)
                .Where(x => x.ParentGroupId == team.Id);

            return new TeamIndex()
            {
                Team = team,
                //Organisation = organisation,
                Projects = _documentSession.Load<Project>(groupAssociations.Select(x => x.ChildGroupId)),
                Avatar = GetAvatar(team)
            };
        }

        private TeamList MakeTeamList(TeamListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Team>()
                //.Customize(x => x.Include<Organisation>(y => y.ParentGroupId == listInput.OrganisationId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList() // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
                .Select(x => new TeamView()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    Website = x.Website,
                    Avatar = GetAvatar(x)
                });

            return new TeamList()
            {
                Organisation = listInput.OrganisationId != null ? _documentSession.Load<Organisation>(listInput.OrganisationId) : null,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Teams = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        //private TeamList MakeTeamListByOrganisationId(TeamListInput listInput)
        //{
        //    RavenQueryStatistics stats;

        //    var results = _documentSession
        //        .Query<Team>()
        //        .Where(x => x.ParentGroupId == listInput.OrganisationId)
        //        .Customize(x => x.Include<Organisation>(y => y.Id == listInput.OrganisationId))
        //        .Statistics(out stats)
        //        .Skip(listInput.Page)
        //        .Take(listInput.PageSize)
        //        .ToList() // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
        //        .Select(x => new TeamView()
        //        {
        //            Id = x.Id,
        //            Description = x.Description,
        //            Name = x.Name,
        //            Website = x.Website,
        //            Avatar = GetAvatar(x)
        //        });

        //    return new TeamList()
        //    {
        //        Organisation = listInput.OrganisationId != null ? _documentSession.Load<Organisation>(listInput.OrganisationId) : null,
        //        Page = listInput.Page,
        //        PageSize = listInput.PageSize,
        //        Teams = results.ToPagedList(
        //            listInput.Page,
        //            listInput.PageSize,
        //            stats.TotalResults,
        //            null)
        //    };
        //}

        private List<TeamView> GetGroupsHavingAddProjectPermission()
        {
            var loggedInUserId = _userContext.GetAuthenticatedUserId();

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => 
                    x.Id.In(_usersGroupsHavingPermissionQuery.GetUsersGroupsHavingPermission(loggedInUserId, "createproject")) &&
                    x.GroupType == "team"
                )
                .ToList()
                .Select(x => new TeamView()
                {
                    Id = x.Id,
                    Description = x.Team.Description,
                    Name = x.Team.Name,
                    Website = x.Team.Website,
                    Avatar = GetAvatar(x.Team)
                })
                .ToList();
        }

        private TeamList MakeTeamListByMembership(TeamListInput listInput)
        {
            RavenQueryStatistics stats;

            var memberships = _documentSession
                .Query<Member>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId)
                .ToList();

            var results = _documentSession
                .Query<Team>()
                .Where(x => x.Id.In(memberships.Select(y => y.Group.Id)))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => new TeamView()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    Website = x.Website,
                    Avatar = GetAvatar(x)
                });

            return new TeamList
            {
                User = listInput.UserId != null ? _documentSession.Load<User>(listInput.UserId) : null,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Teams = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private Avatar GetAvatar(Team team)
        {
            return new Avatar()
            {
                AltTag = team.Description,
                UrlToImage = team.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(team.Avatar.Id, "image", "avatar", team.Avatar.Metadata["metatype"]) :
                    AvatarUris.DefaultTeam
            };
        }

        #endregion
    }
}