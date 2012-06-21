/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;
using System.Collections;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;
using System.Linq;
using System;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class ProjectsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IProjectsViewModelBuilder _projectsViewModelBuilder;
        private readonly ITeamsViewModelBuilder _teamsViewModelBuilder;
        private readonly IStreamItemsViewModelBuilder _streamItemsViewModelBuilder;
        private readonly IObservationsViewModelBuilder _observationsViewModelBuilder;
        private readonly IPostsViewModelBuilder _postsViewModelBuilder;
        private readonly IReferenceSpeciesViewModelBuilder _referenceSpeciesViewModelBuilder;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IProjectsViewModelBuilder projectsViewModelBuilder,
            ITeamsViewModelBuilder teamsViewModelBuilder,
            IStreamItemsViewModelBuilder streamItemsViewModelBuilder,
            IObservationsViewModelBuilder observationsViewModelBuilder,
            IPostsViewModelBuilder postsViewModelBuilder,
            IReferenceSpeciesViewModelBuilder referenceSpeciesViewModelBuilder,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectsViewModelBuilder, "projectsViewModelBuilder");
            Check.RequireNotNull(teamsViewModelBuilder, "teamsViewModelBuilder");
            Check.RequireNotNull(streamItemsViewModelBuilder, "streamItemsViewModelBuilder");
            Check.RequireNotNull(observationsViewModelBuilder, "observationsViewModelBuilder");
            Check.RequireNotNull(postsViewModelBuilder, "postsViewModelBuilder");
            Check.RequireNotNull(referenceSpeciesViewModelBuilder, "referenceSpeciesViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _projectsViewModelBuilder = projectsViewModelBuilder;
            _teamsViewModelBuilder = teamsViewModelBuilder;
            _streamItemsViewModelBuilder = streamItemsViewModelBuilder;
            _observationsViewModelBuilder = observationsViewModelBuilder;
            _postsViewModelBuilder = postsViewModelBuilder;
            _referenceSpeciesViewModelBuilder = referenceSpeciesViewModelBuilder;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Activity(StreamInput streamInput, PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var projectId = "projects/".AppendWith(pagingInput.Id);

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = _streamItemsViewModelBuilder.BuildGroupStreamItems(projectId, streamInput, pagingInput)
                });
            }

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId),
                StreamItems = _streamItemsViewModelBuilder.BuildGroupStreamItems(projectId, null, pagingInput)
            };

            ViewBag.PrerenderedView = "projects"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Observations(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var projectId = "projects/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId),
                Observations = _observationsViewModelBuilder.BuildGroupObservationList(pagingInput)
            };

            if(Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = ViewBag.Model
                });
            }

            ViewBag.PrerenderedView = "observations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult ReferenceSpecies(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var projectId = "projects/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId),
                ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildGroupReferenceSpeciesList(pagingInput)
            };

            ViewBag.PrerenderedView = "referencespecies"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Posts(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var projectId = "projects/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId),
                Posts = _postsViewModelBuilder.BuildGroupPostList(pagingInput)
            };

            ViewBag.PrerenderedView = "posts"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Members(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var projectId = "projects/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId),
                Members = _projectsViewModelBuilder.BuildProjectUserList(pagingInput)
            };

            ViewBag.PrerenderedView = "members"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult About(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var projectId = "projects/".AppendWith(idInput.Id);

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId),
            };

            return View(Form.About);
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            ViewBag.Model = new
            {
                Projects = _projectsViewModelBuilder.BuildProjectList(pagingInput)
            };

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = ViewBag.Model
                });
            }

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var projectId = "projects/".AppendWith(idInput.Id);

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId)
            };

            return new JsonNetResult(new
            {
                Model = ViewBag.Model
            });

            //return View(Form.Index)
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            return new JsonNetResult(_projectsViewModelBuilder.BuildProjectList(pagingInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, idInput.Id ?? Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(),
                Teams = GetTeams(_userContext.GetAuthenticatedUserId())
            };

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new { Model = ViewBag.Model });
            }

            ViewBag.PrerenderedView = "projects";

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var projectId = "projects/".AppendWith(idInput.Id);

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateProject, projectId))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(projectId),
                Teams = GetTeams(_userContext.GetAuthenticatedUserId())
            };

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new { Model = ViewBag.Model });
            }

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var projectId = "projects/".AppendWith(idInput.Id);

            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, projectId))
            {
                return HttpUnauthorized();
            }

            ViewBag.Project = _projectsViewModelBuilder.BuildProject(projectId);

            return View(Form.Delete);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(IdInput idInput)
        {
            DebugToClient(string.Format("SERVER: Projects/Join: id:{0}", idInput.Id));

            Check.RequireNotNull(idInput, "idInput");

            var projectId = idInput.Id.Contains("/") ? idInput.Id : "projects/".AppendWith(idInput.Id);

            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new MemberCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId,
                    CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { RoleNames.ProjectMember }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Leave(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var projectId = "projects/".AppendWith(idInput.Id);

            if (!_userContext.HasGroupPermission(PermissionNames.LeaveProject, projectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new MemberDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            var groupId = createInput.Team != null ? createInput.Team.PrependWith("teams/") : Constants.AppRootId;

            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, groupId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ProjectCreateCommand()
                {
                    Description = createInput.Description,
                    Name = createInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = createInput.Avatar,
                    TeamId = groupId
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(ProjectUpdateInput updateInput)
        {
            Check.RequireNotNull(updateInput, "updateInput");

            var projectId = "projects/".AppendWith(updateInput.Id);

            if (!_userContext.HasGroupPermission<Project>(PermissionNames.UpdateProject, projectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ProjectUpdateCommand()
                {
                    Id = projectId,
                    Description = updateInput.Description,
                    Name = updateInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = updateInput.AvatarId,
                    TeamId = updateInput.TeamId ?? Constants.AppRootId
                });

            return JsonSuccess();
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
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ProjectDeleteCommand()
                {
                    Id = deleteInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private IEnumerable GetTeams(string userId, string projectId = "")
        {
            var teamIds = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == userId)
                .ToList();

            DebugToClient("SERVER: ProjectsController/GetTeams");
            DebugToClient("-----------------------------------");

            DebugToClient(teamIds);

            if (teamIds.Count > 0)
            {
                var teamIdsToQuery = teamIds.SelectMany(x => x.Members.Where(y => y.Group.GroupType == "team").Select(y => y.Group.Id));

                DebugToClient(teamIdsToQuery);

                var teams = _documentSession.Load<Team>(teamIdsToQuery);

                var project = _documentSession.Load<Project>("projects/" + projectId);
                Func<Team, bool> isSelected = null;

                if (project != null)
                {
                    isSelected = x => { return project.Ancestry.Any(y => y.Id == x.Id); };
                }
                else
                {
                    isSelected = x => { return false; };
                }

                return from team in teams
                       select new
                                  {
                                      Text = team.Name,
                                      Value = team.ShortId(),
                                      Selected = isSelected(team)
                                  };
            }
            else // user doesn't have any teams yet
            {
                return null;
            }
        }

        #endregion
    }
}