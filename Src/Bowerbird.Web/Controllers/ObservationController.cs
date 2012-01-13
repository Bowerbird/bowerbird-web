/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Web.Controllers
{
    #region Namespaces

    using System;
    using System.Web.Mvc;

    using Bowerbird.Core;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.Config;

    #endregion

    public class ObservationController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IViewModelRepository _viewModelRepository;

        #endregion

        #region Constructors

        public ObservationController(
            ICommandProcessor commandProcessor,
            IViewModelRepository viewModelRepository
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(viewModelRepository, "viewModelRepository");

            _commandProcessor = commandProcessor;
            _viewModelRepository = viewModelRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        //[HttpGet]
        //public ActionResult Index(ObservationIndexInput observationIndexInput)
        //{
        //    return View(_viewModelRepository.Load<ObservationIndexInput, ObservationIndex>(observationIndexInput));
        //}

        [HttpGet]
        public ActionResult List(int? id, int? page, int? pageSize)
        {
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost] 
        public ActionResult Create(ObservationCreateInput observationCreateInput)
        {
            _commandProcessor.Process(MakeObservationCreateCommand(observationCreateInput));

            return Json("success"); // TODO: Return something more meaningful?
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(ObservationUpdateInput observationUpdateInput)
        {
            throw new NotImplementedException();
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ObservationDeleteInput observationDeleteInput)
        {
            throw new NotImplementedException();
        }

        public ObservationCreateCommand MakeObservationCreateCommand(ObservationCreateInput observationCreateInput)
        {
            Check.RequireNotNull(observationCreateInput, "observationCreateInput");

            return new ObservationCreateCommand()
            {
                Title = observationCreateInput.Title,
                Latitude = observationCreateInput.Latitude,
                Longitude = observationCreateInput.Longitude,
                Address = observationCreateInput.Address,
                IsIdentificationRequired = observationCreateInput.IsIdentificationRequired,
                MediaResources = observationCreateInput.MediaResources,
                ObservationCategory = observationCreateInput.ObservationCategory,
                ObservedOn = observationCreateInput.ObservedOn,
                UserId = observationCreateInput.UserId
            };
        }

        #endregion      
    }
}