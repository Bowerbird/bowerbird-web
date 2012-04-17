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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Raven.Client.Linq;
using System;
using Bowerbird.Core.Config;
using Nustache.Mvc;

namespace Bowerbird.Web.Controllers
{
    public class ObservationController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IObservationViewFactory _observationViewFactory;
        private readonly IBrowseItemFactory _browseItemFactory;
        private readonly IStreamItemFactory _streamItemFactory;

        #endregion

        #region Constructors

        public ObservationController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IUserContext userContext,
            IObservationViewFactory observationViewFactory,
            IBrowseItemFactory browseItemFactory,
            IStreamItemFactory streamItemFactory
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(observationViewFactory, "observationViewFactory");
            Check.RequireNotNull(browseItemFactory, "browseItemFactory");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
            _userContext = userContext;
            _observationViewFactory = observationViewFactory;
            _browseItemFactory = browseItemFactory;
            _streamItemFactory = streamItemFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (_userContext.IsUserAuthenticated() || Request.IsAjaxRequest())
            {
                return Json(MakeObservationIndex(idInput));
            }

            return View(MakeObservationIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ObservationListInput observationListInput)
        {
            if (observationListInput.GroupId != null)
            {
                return Json(MakeObservationListByProjectId(observationListInput), JsonRequestBehavior.AllowGet);
            }

            if (observationListInput.CreatedByUserId != null)
            {
                return Json(MakeObservationListByCreatedByUserId(observationListInput), JsonRequestBehavior.AllowGet);
            }

            return Json(MakeObservationList(observationListInput));
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult Observations()
        {
            ViewData["Observations"] = MakeObservationList();
            var viewResult = View("observationList");
            viewResult.ViewEngineCollection = new ViewEngineCollection { new NustacheViewEngine() };

            return viewResult;
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationCreateInput observationCreateInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(MakeObservationCreateCommand(observationCreateInput));

            return Json("success"); // TODO: Return something more meaningful?
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationUpdateInput observationUpdateInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationUpdateInput.ObservationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(MakeObservationUpdateCommand(observationUpdateInput));

            return Json("success"); // TODO: Return something more meaningful?                
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.DeleteObservation, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(MakeObservationDeleteCommand(idInput));

            return Json("success"); // TODO: Return something more meaningful?
        }

        private ObservationIndex MakeObservationIndex(IdInput idInput)
        {
            return new ObservationIndex()
            {
                Observation = _documentSession.Load<Observation>(idInput.Id)
            };
        }

        private IEnumerable<StreamItem> MakeObservationList()
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.ContributionType.Equals("observation"))
                .OrderByDescending(x => x.CreatedDateTime)
                .Take(10)
                .ToList()
                .ToPagedList(1, 10, stats.TotalResults)
                .PagedListItems
                .Select(MakeStreamItem);
        }

        private StreamItem MakeStreamItem(All_GroupContributions.Result groupContributionResult)
        {
            object item = null;
            string description = null;
            IEnumerable<string> groups = null;

            switch (groupContributionResult.ContributionType)
            {
                case "Observation":
                    item = _observationViewFactory.Make(groupContributionResult.Observation);
                    description = groupContributionResult.Observation.User.FirstName + " added an observation";
                    groups = groupContributionResult.Observation.Groups.Select(x => x.GroupId);
                    break;
            }

            return _streamItemFactory.Make(
                item,
                groups,
                "observation",
                groupContributionResult.GroupUser,
                groupContributionResult.GroupCreatedDateTime,
                description);
        }


        private ObservationList MakeObservationList(ObservationListInput observationListInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray();

            return new ObservationList
            {
                Page = observationListInput.Page,
                PageSize = observationListInput.PageSize,
                Observations = observations.ToPagedList(
                    observationListInput.Page,
                    observationListInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private ObservationList MakeObservationListByProjectId(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Where(x => x.Groups.Any(y => y.GroupId == listInput.GroupId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            return new ObservationList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Project = listInput.GroupId != null ? _documentSession.Load<Project>(listInput.GroupId) : null,
                Observations = observations.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private ObservationList MakeObservationListByCreatedByUserId(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Customize(x => x.Include(listInput.CreatedByUserId))
                .Where(x => x.User.Id == listInput.CreatedByUserId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray();

            return new ObservationList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                CreatedByUser = _documentSession.Load<User>(listInput.CreatedByUserId),
                Observations = observations.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private ObservationCreateCommand MakeObservationCreateCommand(ObservationCreateInput observationCreateInput)
        {
            return new ObservationCreateCommand()
                       {
                           Title = observationCreateInput.Title,
                           Latitude = observationCreateInput.Latitude,
                           Longitude = observationCreateInput.Longitude,
                           Address = observationCreateInput.Address,
                           IsIdentificationRequired = observationCreateInput.IsIdentificationRequired,
                           Category = observationCreateInput.Category,
                           ObservedOn = observationCreateInput.ObservedOn,
                           UserId = _userContext.GetAuthenticatedUserId(),
                           Projects = observationCreateInput.Projects,
                           AddMedia = observationCreateInput.AddMedia.Select(x => new Tuple<string, string, string>(x.MediaResourceId, x.Description, x.Licence))
                       };
        }

        private ObservationUpdateCommand MakeObservationUpdateCommand(ObservationUpdateInput observationUpdateInput)
        {
            return new ObservationUpdateCommand
                       {
                           Id = observationUpdateInput.ObservationId,
                           Title = observationUpdateInput.Title,
                           Latitude = observationUpdateInput.Latitude,
                           Longitude = observationUpdateInput.Longitude,
                           Address = observationUpdateInput.Address,
                           IsIdentificationRequired = observationUpdateInput.IsIdentificationRequired,
                           Category = observationUpdateInput.Category,
                           ObservedOn = observationUpdateInput.ObservedOn,
                           UserId = _userContext.GetAuthenticatedUserId(),
                           Projects = observationUpdateInput.Projects
                       };
        }

        private ObservationDeleteCommand MakeObservationDeleteCommand(IdInput idInput)
        {
            return new ObservationDeleteCommand
                       {
                           ObservationId =  idInput.Id,
                           UserId = _userContext.GetAuthenticatedUserId()
                       };
        }

        #endregion
    }
}