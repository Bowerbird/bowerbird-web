///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Web.Mvc;
//using Bowerbird.Core.Commands;
//using Bowerbird.Core.Config;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Web.Builders;
//using Bowerbird.Web.Config;
//using Bowerbird.Web.ViewModels;

//namespace Bowerbird.Web.Controllers
//{
//    [Restful]
//    public class ReferenceSpeciesController : ControllerBase
//    {
//        #region Members

//        private readonly ICommandProcessor _commandProcessor;
//        private readonly IUserContext _userContext;
//        private readonly IReferenceSpeciesViewModelBuilder _referenceSpeciesViewModelBuilder;

//        #endregion

//        #region Constructors

//        public ReferenceSpeciesController(
//            ICommandProcessor commandProcessor,
//            IUserContext userContext,
//            IReferenceSpeciesViewModelBuilder referenceSpeciesViewModelBuilder
//            )
//        {
//            Check.RequireNotNull(commandProcessor, "commandProcessor");
//            Check.RequireNotNull(userContext, "userContext");
//            Check.RequireNotNull(referenceSpeciesViewModelBuilder, "referenceSpeciesViewModelBuilder");

//            _commandProcessor = commandProcessor;
//            _userContext = userContext;
//            _referenceSpeciesViewModelBuilder = referenceSpeciesViewModelBuilder;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        [HttpGet]
//        public ActionResult Index(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            ViewBag.ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildReferenceSpecies(idInput);

//            return View(Form.Index);
//        }

//        [HttpGet]
//        public ActionResult GetOne(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            return new JsonNetResult(_referenceSpeciesViewModelBuilder.BuildReferenceSpecies(idInput));
//        }

//        [HttpGet]
//        public ActionResult GetMany(PagingInput pagingInput)
//        {
//            Check.RequireNotNull(pagingInput, "pagingInput");

//            return new JsonNetResult(_referenceSpeciesViewModelBuilder.BuildReferenceSpeciesList(pagingInput));
//        }

//        [HttpGet]
//        public ActionResult CreateForm(IdInput idInput)
//        {
//            Check.Require(idInput != null && idInput.Id != null, "No Group has been supplied for new Reference Species");

//            if (!_userContext.HasGroupPermission(PermissionNames.CreateReferenceSpecies, idInput.Id))
//            {
//                return HttpUnauthorized();
//            }

//            return View(Form.Create);
//        }

//        [HttpGet]
//        [Authorize]
//        public ActionResult UpdateForm(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            //if (!_userContext.HasGroupPermission<ReferenceSpecies>(PermissionNames.UpdateReferenceSpecies, idInput.Id))
//            //{
//            //    return HttpUnauthorized();
//            //}

//            ViewBag.ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildReferenceSpecies(idInput);

//            return View(Form.Update);
//        }

//        [HttpGet]
//        [Authorize]
//        public ActionResult DeleteForm(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            //if (!_userContext.HasGroupPermission<ReferenceSpecies>(PermissionNames.DeleteReferenceSpecies, idInput.Id))
//            //{
//            //    return HttpUnauthorized();
//            //}

//            ViewBag.ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildReferenceSpecies(idInput);

//            return View(Form.Delete);
//        }

//        [HttpPost]
//        [Authorize]
//        public ActionResult Create(ReferenceSpeciesCreateInput createInput)
//        {
//            Check.RequireNotNull(createInput, "createInput");

//            if (!_userContext.HasGroupPermission(PermissionNames.CreateReferenceSpecies, createInput.GroupId ?? Constants.AppRootId))
//            {
//                return HttpUnauthorized();
//            }

//            if (!ModelState.IsValid)
//            {
//                return JsonFailed();
//            }

//            _commandProcessor.Process(
//                new ReferenceSpeciesCreateCommand()
//                {
//                    SpeciesId = createInput.SpeciesId,
//                    GroupId = createInput.GroupId,
//                    CreatedOn = createInput.CreatedOn,
//                    UserId = _userContext.GetAuthenticatedUserId(),
//                    SmartTags = createInput.SmartTags
//                });

//            return JsonSuccess();
//        }

//        [HttpPut]
//        [Authorize]
//        [Transaction]
//        public ActionResult Update(ReferenceSpeciesUpdateInput updateInput)
//        {
//            Check.RequireNotNull(updateInput, "updateInput");

//            //if (!_userContext.HasGroupPermission<ReferenceSpecies>(PermissionNames.DeleteReferenceSpecies, updateInput.Id))
//            //{
//            //    return HttpUnauthorized();
//            //}

//            if (!ModelState.IsValid)
//            {
//                return JsonFailed();
//            }

//            _commandProcessor.Process(
//                new ReferenceSpeciesUpdateCommand()
//                {
//                    Id = updateInput.Id,
//                    SpeciesId = updateInput.SpeciesId,
//                    GroupId = updateInput.GroupId,
//                    ModifiedOn = updateInput.UpdatedOn,
//                    UserId = _userContext.GetAuthenticatedUserId(),
//                    SmartTags = updateInput.SmartTags
//                });

//            return JsonSuccess();
//        }

//        [HttpDelete]
//        [Authorize]
//        public ActionResult Delete(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            //if (!_userContext.HasGroupPermission<ReferenceSpecies>(PermissionNames.DeleteReferenceSpecies, idInput.Id))
//            //{
//            //    return HttpUnauthorized();
//            //}

//            if (!ModelState.IsValid)
//            {
//                return JsonFailed();
//            }

//            _commandProcessor.Process(
//                new ReferenceSpeciesDeleteCommand
//                {
//                    Id = idInput.Id,
//                    UserId = _userContext.GetAuthenticatedUserId()
//                });

//            return JsonSuccess();
//        }

//        #endregion
//    }
//}