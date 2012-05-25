/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;
using System;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class TeamsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly ITeamsViewModelBuilder _teamsViewModelBuilder;
        private readonly IStreamItemsViewModelBuilder _streamItemsViewModelBuilder;
        private readonly IProjectsViewModelBuilder _projectsViewModelBuilder;
        private readonly IPostsViewModelBuilder _postsViewModelBuilder;
        private readonly IReferenceSpeciesViewModelBuilder _referenceSpeciesViewModelBuilder;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            ITeamsViewModelBuilder teamsViewModelBuilder,
            IStreamItemsViewModelBuilder streamItemsViewModelBuilder,
            IProjectsViewModelBuilder projectsViewModelBuilder,
            IPostsViewModelBuilder postsViewModelBuilder,
            IReferenceSpeciesViewModelBuilder referenceSpeciesViewModelBuilder,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(teamsViewModelBuilder, "teamsViewModelBuilder");
            Check.RequireNotNull(streamItemsViewModelBuilder, "streamItemsViewModelBuilder");
            Check.RequireNotNull(projectsViewModelBuilder, "projectsViewModelBuilder");
            Check.RequireNotNull(postsViewModelBuilder, "postsViewModelBuilder");
            Check.RequireNotNull(referenceSpeciesViewModelBuilder, "referenceSpeciesViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _teamsViewModelBuilder = teamsViewModelBuilder;
            _streamItemsViewModelBuilder = streamItemsViewModelBuilder;
            _projectsViewModelBuilder = projectsViewModelBuilder;
            _postsViewModelBuilder = postsViewModelBuilder;
            _referenceSpeciesViewModelBuilder = referenceSpeciesViewModelBuilder;
            _documentSession = documentSession;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Stream(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Team = _teamsViewModelBuilder.BuildTeam(new IdInput() { Id = "teams/" + pagingInput.Id }),
                StreamItems = _streamItemsViewModelBuilder.BuildGroupStreamItems(pagingInput)
            };

            ViewBag.PrerenderedView = "teams"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult StreamList(PagingInput pagingInput)
        {
            return new JsonNetResult(_streamItemsViewModelBuilder.BuildGroupStreamItems(pagingInput));
        }

        [HttpGet]
        public ActionResult ReferenceSpecies(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Team = _teamsViewModelBuilder.BuildTeam(new IdInput() { Id = "teams/" + pagingInput.Id }),
                ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildGroupReferenceSpeciesList(pagingInput)
            };

            ViewBag.PrerenderedView = "referencespecies"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Projects(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Team = _teamsViewModelBuilder.BuildTeam(new IdInput() { Id = "teams/" + pagingInput.Id }),
                Projects = _projectsViewModelBuilder.BuildTeamProjectList(pagingInput)
            };

            ViewBag.PrerenderedView = "projects"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Posts(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Team = _teamsViewModelBuilder.BuildTeam(new IdInput() { Id = "teams/" + pagingInput.Id }),
                Posts = _postsViewModelBuilder.BuildGroupPostList(pagingInput)
            };

            ViewBag.PrerenderedView = "posts"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Members(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Team = _teamsViewModelBuilder.BuildTeam(new IdInput() { Id = "teams/" + pagingInput.Id }),
                Members = _teamsViewModelBuilder.BuildTeamUserList(pagingInput)
            };

            ViewBag.PrerenderedView = "members"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult About()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            ViewBag.TeamList = _teamsViewModelBuilder.BuildTeamList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return new JsonNetResult(_teamsViewModelBuilder.BuildTeam(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            return new JsonNetResult(_teamsViewModelBuilder.BuildTeamList(pagingInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateTeam, idInput.Id ?? Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new
            {
                Team = _teamsViewModelBuilder.BuildNewTeam(),
                Organisations = GetOrganisations(_userContext.GetAuthenticatedUserId())
            };

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new {Model = ViewBag.Model});
            }

            ViewBag.PrerenderedView = "teams";

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateTeam))
            {
                return HttpUnauthorized();
            }

            ViewBag.Team = _teamsViewModelBuilder.BuildTeam(idInput);

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteTeam))
            {
                return HttpUnauthorized();
            }

            ViewBag.Team = _teamsViewModelBuilder.BuildTeam(idInput);

            return View(Form.Delete);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.JoinTeam, idInput.Id))
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
                    GroupId = idInput.Id,
                    CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { RoleNames.TeamMember }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Leave(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.LeaveTeam, idInput.Id))
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
                    GroupId = idInput.Id
                });

            return JsonSuccess();
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddProject(GroupAssociationCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            if (!_userContext.HasGroupPermission<Team>(PermissionNames.AddTeam, createInput.ParentGroupId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new GroupAssociationCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    ParentGroupId = createInput.ParentGroupId,
                    ChildGroupId = createInput.ChildGroupId
                });

            return JsonSuccess();
        }

        [HttpPost]
        [Authorize]
        public ActionResult RemoveProject(GroupAssociationDeleteInput deleteInput)
        {
            Check.RequireNotNull(deleteInput, "deleteInput");

            if (!_userContext.HasGroupPermission<Team>(PermissionNames.RemoveTeam, deleteInput.ParentGroupId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new GroupAssociationDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    ParentGroupId = deleteInput.ParentGroupId,
                    ChildGroupId = deleteInput.ChildGroupId
                });

            return JsonSuccess();
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
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamCreateCommand()
                    {
                        Description = createInput.Description,
                        Name = createInput.Name,
                        UserId = _userContext.GetAuthenticatedUserId(),
                        OrganisationId = createInput.Organisation
                    }
                );

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(TeamUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.UpdateTeam, updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamUpdateCommand()
                {
                    Id = updateInput.Id,
                    Description = updateInput.Description,
                    Name = updateInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = updateInput.AvatarId
                }
            );

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteTeam, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamDeleteCommand()
                {
                    Id = deleteInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
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
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamProjectCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Name = projectCreateInput.Name,
                    Description = projectCreateInput.Description,
                    Administrators = teamProjectCreateInput.Administrators,
                    Members = teamProjectCreateInput.Members,
                    TeamId = teamProjectCreateInput.TeamId
                }
            );

            return JsonSuccess();
        }

        private IEnumerable GetOrganisations(string userId, string teamId = "")
        {
            var organisationIds = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == userId)
                .ToList()
                .SelectMany(x => x.Members.Where(y => y.Group.GroupType == "organisation").Select(y => y.Group.Id));

            var organisations = _documentSession.Load<Organisation>(organisationIds);

            var team = _documentSession.Load<Team>("teams/" + teamId);
            Func<Organisation, bool> isSelected = null;

            if (team != null)
            {
                isSelected = x => { return team.Ancestry.Any(y => y.Id == x.Id); };
            }
            else
            {
                isSelected = x => { return false; };
            }

            return from organisation in organisations
                   select new
                   {
                       Text = organisation.Name,
                       Value = organisation.ShortId(),
                       Selected = isSelected(organisation)
                   };
        }

        #endregion
    }
}