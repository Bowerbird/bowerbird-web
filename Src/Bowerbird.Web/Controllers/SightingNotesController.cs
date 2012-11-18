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
using System.Dynamic;
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
using System.Collections;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class SightingNotesController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly ISightingNoteViewModelBuilder _sightingNoteViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IPermissionManager _permissionManager;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SightingNotesController(
            IMessageBus messageBus,
            IUserContext userContext,
            ISightingNoteViewModelBuilder sightingNoteViewModelBuilder,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IPermissionManager permissionManager,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(sightingNoteViewModelBuilder, "sightingNoteViewModelBuilder");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(documentSession, "documentSession");

            _messageBus = messageBus;
            _userContext = userContext;
            _sightingNoteViewModelBuilder = sightingNoteViewModelBuilder;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _permissionManager = permissionManager;
            _documentSession = documentSession;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, idInput.Id))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.SightingNote = _sightingNoteViewModelBuilder.BuildNewSightingNote(idInput.Id);
            viewModel.Sighting = _sightingViewModelBuilder.BuildSighting(idInput.Id);
            viewModel.DescriptionTypesSelectList = GetDescriptionTypesSelectList();
            //viewModel.ProjectsSelectList = GetProjectsSelectList(projectId);
            //viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel,
                "sightingnotes",
                "create",
                new Action<dynamic>(x => x.Model.Create = true));
        }

        //[HttpGet]
        //[Authorize]
        //public ActionResult UpdateForm(IdInput idInput)
        //{
        //    if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
        //    {
        //        return HttpUnauthorized();
        //    }

        //    ViewBag.Observation = _observationsViewModelBuilder.BuildObservation(idInput);

        //    ViewBag.ObservationNote = _observationNotesViewModelBuilder.BuildObservationNote(idInput);

        //    return View(Form.Update);
        //}

        //[HttpGet]
        //[Authorize]
        //public ActionResult DeleteForm(IdInput idInput)
        //{
        //    if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
        //    {
        //        return HttpUnauthorized();
        //    }

        //    ViewBag.Observation = _observationsViewModelBuilder.BuildObservation(idInput);

        //    ViewBag.ObservationNote = _observationNotesViewModelBuilder.BuildObservationNote(idInput);

        //    return View(Form.Delete);
        //}

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Create(SightingNoteCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, createInput.SightingId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new SightingNoteCreateCommand()
                    {
                        SightingId = createInput.SightingId,
                        NotedOn = DateTime.UtcNow,
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
                        CommonGroupNames = createInput.CommonGroupNames ?? new string[] {},
                        CommonNames =  createInput.CommonNames ?? new string[] {}
                    });

            return JsonSuccess();
        }

        //[Transaction]
        //[HttpPut]
        //[Authorize]
        //public ActionResult Update(ObservationNoteUpdateInput updateInput)
        //{
        //    if (!_userContext.HasGroupPermission<ObservationNote>(PermissionNames.UpdateObservation, updateInput.Id))
        //    {
        //        return HttpUnauthorized();
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return JsonFailed();
        //    }

        //    _commandProcessor.Process(
        //        new ObservationNoteUpdateCommand
        //        {
        //            Id = updateInput.Id,
        //            NotedOn = updateInput.NotedOn,
        //            UserId = _userContext.GetAuthenticatedUserId(),
        //            CommonName = updateInput.CommonName,
        //            ScientificName = updateInput.ScientificName,
        //            Taxonomy = updateInput.Taxonomy,
        //            Descriptions = updateInput.Descriptions,
        //            References = updateInput.References,
        //            Tags = updateInput.Tags
        //        });

        //    return JsonSuccess();
        //}

        //[Transaction]
        //[HttpDelete]
        //[Authorize]
        //public ActionResult Delete(IdInput idInput)
        //{
        //    if (!_userContext.HasGroupPermission<ObservationNote>(PermissionNames.DeleteObservationNote, idInput.Id))
        //    {
        //        return HttpUnauthorized();
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return JsonFailed();
        //    }

        //    _commandProcessor.Process(
        //        new ObservationNoteDeleteCommand
        //        {
        //            Id = idInput.Id,
        //            UserId = _userContext.GetAuthenticatedUserId()
        //        });

        //    return JsonSuccess();
        //}

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