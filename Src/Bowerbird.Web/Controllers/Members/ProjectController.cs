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
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using System;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers.Members
{
    public class ProjectController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public ProjectController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
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
            //if (listInput.TeamId != null)
            //{
            //    return Json(MakeProjectListByTeamId(listInput), JsonRequestBehavior.AllowGet);
            //}

            if (listInput.UserId != null)
            {
                return Json(MakeProjectListByMembership(listInput), JsonRequestBehavior.AllowGet);
            }

            return Json(MakeProjectList(listInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, createInput.TeamId ?? Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeProjectCreateCommand(createInput));

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(ProjectUpdateInput updateInput) 
        {
            if (!_userContext.HasGroupPermission<Project>(PermissionNames.UpdateProject, updateInput.ProjectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }
            
            _commandProcessor.Process(MakeProjectUpdateCommand(updateInput));

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasGroupPermission<Project>(PermissionNames.DeleteProject, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeProjectDeleteCommand(deleteInput));

            return Json("Success");
        }

        private ProjectCreateCommand MakeProjectCreateCommand(ProjectCreateInput createInput)
        {
            return new ProjectCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectDeleteCommand MakeProjectDeleteCommand(IdInput deleteInput)
        {
            return new ProjectDeleteCommand()
            {
                Id = deleteInput.Id,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectUpdateCommand MakeProjectUpdateCommand(ProjectUpdateInput updateInput)
        {
            return new ProjectUpdateCommand()
            {
                Description = updateInput.Description,
                Name = updateInput.Name,
                UserId = _userContext.GetAuthenticatedUserId(),
                AvatarId = updateInput.AvatarId
            };
        }

        protected ProjectIndex MakeProjectIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession.Load<Project>(idInput.Id);

            return new ProjectIndex()
            {
                Project = project,
                Avatar = GetAvatar(project),
                //Team = project.ParentGroupId != null ? _documentSession.Load<Team>(project.ParentGroupId) : null
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
                .ToList()
                .Select(x => new ProjectView()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    Website = x.Website,
                    Avatar = GetAvatar(x)
                });

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

        //protected ProjectList MakeProjectListByTeamId(ProjectListInput listInput)
        //{
        //    RavenQueryStatistics stats;

        //    var results = _documentSession
        //        .Query<Project>()
        //        .Where(x => x.ParentGroupId == listInput.TeamId)
        //        .Customize(x => x.Include<Team>(y => y.Id == listInput.TeamId))
        //        .Statistics(out stats)
        //        .Skip(listInput.Page)
        //        .Take(listInput.PageSize)
        //        .ToList()
        //        .Select(x => new ProjectView()
        //        {
        //            Id = x.Id,
        //            Description = x.Description,
        //            Name = x.Name,
        //            Website = x.Website,
        //            Avatar = GetAvatar(x)
        //        });

        //    return new ProjectList
        //    {
        //        Team = listInput.TeamId != null ? _documentSession.Load<Team>(listInput.TeamId) : null,
        //        Page = listInput.Page,
        //        PageSize = listInput.PageSize,
        //        Projects = results.ToPagedList(
        //            listInput.Page,
        //            listInput.PageSize,
        //            stats.TotalResults,
        //            null)
        //    };
        //}

        protected ProjectList MakeProjectListByMembership(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var memberships = _documentSession
                .Query<Member>()
                .Where(x => x.User.Id == listInput.UserId);

            var results = _documentSession
                .Query<Project>()
                .Where(x => x.Id.In(memberships.Select(y => y.Group.Id)))
                .Customize(x => x.Include<User>(y => y.Id == listInput.UserId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => new ProjectView()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    Website = x.Website,
                    Avatar = GetAvatar(x)
                });

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

        private Avatar GetAvatar(Project project)
        {
            return new Avatar()
            {
                AltTag = project.Description,
                UrlToImage = project.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(project.Avatar.Id, "image", "avatar", project.Avatar.Metadata["metatype"]) :
                    _configService.GetDefaultAvatar("project")
            };
        }

        #endregion
    }
}