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
using Bowerbird.Core.Infrastructure;
using Bowerbird.Web.Builders;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using System;
using Bowerbird.Core.Config;
using Raven.Client;
using System.Collections;
using System.Dynamic;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class ObservationsController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IDocumentSession _documentSession;
        private readonly IPermissionManager _permissionManager;
        private readonly ISightingNoteViewModelBuilder _sightingNoteViewModelBuilder;

        #endregion

        #region Constructors

        public ObservationsController(
            IMessageBus messageBus,
            IUserContext userContext,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IDocumentSession documentSession,
            IPermissionManager permissionManager,
            ISightingNoteViewModelBuilder sightingNoteViewModelBuilder
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(sightingNoteViewModelBuilder, "sightingNoteViewModelBuilder");

            _messageBus = messageBus;
            _userContext = userContext;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _documentSession = documentSession;
            _permissionManager = permissionManager;
            _sightingNoteViewModelBuilder = sightingNoteViewModelBuilder;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Observation = _sightingViewModelBuilder.BuildSighting(observationId);

            return RestfulResult(
                viewModel,
                "observations",
                "index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string category = "", string projectId = "")
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!string.IsNullOrWhiteSpace(projectId))
            {
                var project = _documentSession.Load<Project>(projectId);

                if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, project.Id))
                {
                    return HttpUnauthorized(); // TODO: Probably should return a soft user error suggesting user joins project
                }
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelBuilder.BuildNewObservation(category, projectId);
            viewModel.CategorySelectList = GetCategorySelectList(null, category);
            viewModel.ProjectsSelectList = GetProjectsSelectList(projectId);
            viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel, 
                "observations", 
                "create", 
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return HttpUnauthorized();
            }

            var observation = _documentSession.Load<Observation>(observationId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelBuilder.BuildSighting(observationId);
            viewModel.CategorySelectList = GetCategorySelectList(observationId);
            viewModel.ProjectsSelectList = GetProjectsSelectList(observation.Groups.Where(x => x.Group.GroupType == "project").Select(x => x.Group.Id).ToArray());
            viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel,
                "observations",
                "update", 
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelBuilder.BuildSighting(observationId);

            return RestfulResult(
                viewModel,
                "observations",
                "delete", 
                new Action<dynamic>(x => x.Model.Delete = true));
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationCreateInput createInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            var key = string.IsNullOrWhiteSpace(createInput.Key) ? Guid.NewGuid().ToString() : createInput.Key;

            _messageBus.Send(new ObservationCreateCommand()
                {
                    Key = key,
                    Title = createInput.Title,
                    Latitude = createInput.Latitude,
                    Longitude = createInput.Longitude,
                    Address = createInput.Address,
                    IsIdentificationRequired = createInput.IsIdentificationRequired,
                    AnonymiseLocation = createInput.AnonymiseLocation,
                    Category = createInput.Category,
                    ObservedOn = createInput.ObservedOn,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Projects = createInput.ProjectIds,
                    Media = createInput.Media.Select(x => new ObservationMediaUpdateCommand()
                        {
                            MediaResourceId = x.MediaResourceId,
                            Key = x.Key,
                            Description = x.Description,
                            Licence = x.Licence,
                            IsPrimaryMedia = x.IsPrimaryMedia
                        })
                });

            if (createInput.Note != null && IsValidSightingNote(createInput.Note))
            {
                _messageBus.Send(
                    new SightingNoteCreateCommand()
                        {
                            SightingKey = key, // We assign this note via the sighting key, rather than Id because we don't have the sighting id yet.
                            UserId = _userContext.GetAuthenticatedUserId(),
                            Descriptions = createInput.Note.Descriptions ?? new Dictionary<string, string>(),
                            Tags = createInput.Note.Tags ?? string.Empty,
                            IsCustomIdentification = createInput.Note.IsCustomIdentification,
                            Taxonomy = createInput.Note.Taxonomy ?? string.Empty,
                            Category = createInput.Note.Category ?? string.Empty,
                            Kingdom = createInput.Note.Kingdom ?? string.Empty,
                            Phylum = createInput.Note.Phylum ?? string.Empty,
                            Class = createInput.Note.Class ?? string.Empty,
                            Order = createInput.Note.Order ?? string.Empty,
                            Family = createInput.Note.Family ?? string.Empty,
                            Genus = createInput.Note.Genus ?? string.Empty,
                            Species = createInput.Note.Species ?? string.Empty,
                            Subspecies = createInput.Note.Subspecies ?? string.Empty,
                            CommonGroupNames = createInput.Note.CommonGroupNames ?? new string[] {},
                            CommonNames = createInput.Note.CommonNames ?? new string[] {}
                        });
            }

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationUpdateInput updateInput)
        {
            string observationId = VerbosifyId<Observation>(updateInput.Id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new ObservationUpdateCommand
                {
                    Id = observationId,
                    Title = updateInput.Title,
                    Latitude = updateInput.Latitude,
                    Longitude = updateInput.Longitude,
                    Address = updateInput.Address,
                    IsIdentificationRequired = updateInput.IsIdentificationRequired,
                    AnonymiseLocation = updateInput.AnonymiseLocation,
                    Category = updateInput.Category,
                    ObservedOn = updateInput.ObservedOn,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Projects = updateInput.ProjectIds,
                    Media = updateInput.Media.Select(x => new ObservationMediaUpdateCommand()
                    {
                        MediaResourceId = x.MediaResourceId,
                        Description = x.Description,
                        Licence = x.Licence,
                        IsPrimaryMedia = x.IsPrimaryMedia
                    })
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new ObservationDeleteCommand
                {
                    Id = id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNoteForm(string id)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, id))
            //{
            //    return HttpUnauthorized();
            //}

            var observationId = VerbosifyId<Observation>(id);

            dynamic viewModel = new ExpandoObject();

            viewModel.SightingNote = _sightingNoteViewModelBuilder.BuildCreateSightingNote(observationId);
            viewModel.Sighting = _sightingViewModelBuilder.BuildSighting(observationId);
            viewModel.DescriptionTypesSelectList = GetDescriptionTypesSelectList();
            viewModel.CategorySelectList = GetCategorySelectList();
            viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel,
                "sightingnotes",
                "createnote",
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateNoteForm(string id, int sightingNoteId)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateSightingNote))
            //{
            //    return HttpUnauthorized();
            //}

            var observationId = VerbosifyId<Observation>(id);

            dynamic viewModel = new ExpandoObject();

            viewModel.SightingNote = _sightingNoteViewModelBuilder.BuildUpdateSightingNote(observationId, sightingNoteId);
            viewModel.Sighting = _sightingViewModelBuilder.BuildSighting(observationId);
            viewModel.DescriptionTypesSelectList = GetDescriptionTypesSelectList();
            viewModel.CategorySelectList = GetCategorySelectList();
            viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel,
                "sightingnotes",
                "updatenote",
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult CreateNote(SightingNoteCreateInput createInput)
        {
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, createInput.SightingId))
            //{
            //    return HttpUnauthorized();
            //}

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new SightingNoteCreateCommand()
                {
                    SightingId = createInput.SightingId,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Descriptions = createInput.Descriptions ?? new Dictionary<string, string>(),
                    Tags = createInput.Tags ?? string.Empty,
                    IsCustomIdentification = createInput.IsCustomIdentification,
                    Taxonomy = createInput.Taxonomy ?? string.Empty,
                    Category = createInput.Category ?? string.Empty,
                    Kingdom = createInput.Kingdom ?? string.Empty,
                    Phylum = createInput.Phylum ?? string.Empty,
                    Class = createInput.Class ?? string.Empty,
                    Order = createInput.Order ?? string.Empty,
                    Family = createInput.Family ?? string.Empty,
                    Genus = createInput.Genus ?? string.Empty,
                    Species = createInput.Species ?? string.Empty,
                    Subspecies = createInput.Subspecies ?? string.Empty,
                    CommonGroupNames = createInput.CommonGroupNames ?? new string[] { },
                    CommonNames = createInput.CommonNames ?? new string[] { }
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult UpdateNote(SightingNoteUpdateInput updateInput)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, updateInput.Id))
            //{
            //    return HttpUnauthorized();
            //}

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new SightingNoteUpdateCommand()
                {
                    Id = updateInput.Id,
                    SightingId = updateInput.SightingId,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Descriptions = updateInput.Descriptions ?? new Dictionary<string, string>(),
                    Tags = updateInput.Tags ?? string.Empty,
                    IsCustomIdentification = updateInput.IsCustomIdentification,
                    Taxonomy = updateInput.Taxonomy ?? string.Empty,
                    Category = updateInput.Category ?? string.Empty,
                    Kingdom = updateInput.Kingdom ?? string.Empty,
                    Phylum = updateInput.Phylum ?? string.Empty,
                    Class = updateInput.Class ?? string.Empty,
                    Order = updateInput.Order ?? string.Empty,
                    Family = updateInput.Family ?? string.Empty,
                    Genus = updateInput.Genus ?? string.Empty,
                    Species = updateInput.Species ?? string.Empty,
                    Subspecies = updateInput.Subspecies ?? string.Empty,
                    CommonGroupNames = updateInput.CommonGroupNames ?? new string[] { },
                    CommonNames = updateInput.CommonNames ?? new string[] { }
                });

            return JsonSuccess();
        }

        private bool IsValidSightingNote(SightingNoteCreateInput sightingNoteCreateInput)
        {
            // At least one of the following items has been filled in:
            return
                sightingNoteCreateInput.Descriptions.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value)).Count() > 0 || // At least one description
                sightingNoteCreateInput.Tags.Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries).Count() > 0 || // At least one tag
                sightingNoteCreateInput.IsCustomIdentification ? true : !string.IsNullOrWhiteSpace(sightingNoteCreateInput.Taxonomy); // An identification
        }

        private IEnumerable GetCategorySelectList(string observationId = "", string category = "")
        {
            if (!string.IsNullOrWhiteSpace(observationId))
            {
                category = _documentSession.Load<Observation>(observationId).Category;
            }

            return _documentSession
                .Load<AppRoot>(Constants.AppRootId)
                .Categories
                .Select(x => new
                   {
                       Text = x.Name,
                       Value = x.Name,
                       Selected = x.Name == category
                   });
        }

        private IEnumerable GetProjectsSelectList(params string[] projectIds)
        {
            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .Single()
                .Projects
                .Select(x => new
                {
                    Text = x.Name,
                    Value = x.Id,
                    Selected = projectIds.Any(y => y == x.Id)
                });
        }

        private IEnumerable GetCategories()
        {
            return _documentSession
                .Load<AppRoot>(Constants.AppRootId)
                .Categories
                .ToList();
        }

        private IEnumerable GetDescriptionTypesSelectList()
        {
            return new List<object>
                {
                    new
                        {
                            Text = "Physical Description",
                            Value = "physicaldescription",
                            Selected = false
                        },
                    new
                        {
                            Text = "Similar Species",
                            Value = "similarspecies",
                            Selected = false
                        },
                    new
                        {
                            Text = "Distribution",
                            Value = "distribution",
                            Selected = false
                        },
                    new
                        {
                            Text = "Habitat",
                            Value = "habitat",
                            Selected = false
                        },
                    new
                        {
                            Text = "Seasonal Variation",
                            Value = "seasonalvariation",
                            Selected = false
                        },
                    new
                        {
                            Text = "Behaviour",
                            Value = "behaviour",
                            Selected = false
                        },
                    new
                        {
                            Text = "Food",
                            Value = "food",
                            Selected = false
                        },
                    new
                        {
                            Text = "Life Cycle",
                            Value = "lifecycle",
                            Selected = false
                        },
                    new
                        {
                            Text = "Conservation Status",
                            Value = "conservationstatus",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Name",
                            Value = "indigenousname",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Use",
                            Value = "indigenoususe",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Stories",
                            Value = "indigenousstories",
                            Selected = false
                        },
                    new
                        {
                            Text = "General Details",
                            Value = "other",
                            Selected = false
                        }
                };
        }

        private IEnumerable GetDescriptionTypes()
        {
            return new List<object>
                {
                    new
                        {
                            Text = "Physical Description",
                            Value = "physicaldescription",
                            Selected = false
                        },
                    new
                        {
                            Text = "Similar Species",
                            Value = "similarspecies",
                            Selected = false
                        },
                    new
                        {
                            Text = "Distribution",
                            Value = "distribution",
                            Selected = false
                        },
                    new
                        {
                            Text = "Habitat",
                            Value = "habitat",
                            Selected = false
                        },
                    new
                        {
                            Text = "Seasonal Variation",
                            Value = "seasonalvariation",
                            Selected = false
                        },
                    new
                        {
                            Text = "Behaviour",
                            Value = "behaviour",
                            Selected = false
                        },
                    new
                        {
                            Text = "Food",
                            Value = "food",
                            Selected = false
                        },
                    new
                        {
                            Text = "Life Cycle",
                            Value = "lifecycle",
                            Selected = false
                        },
                    new
                        {
                            Text = "Conservation Status",
                            Value = "conservationstatus",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Name",
                            Value = "indigenousname",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Use",
                            Value = "indigenoususe",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Stories",
                            Value = "indigenousstories",
                            Selected = false
                        },
                    new
                        {
                            Text = "General Details",
                            Value = "other",
                            Selected = false
                        }
                };
        }

        #endregion
    }
}