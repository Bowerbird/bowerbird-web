using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Entities;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Controllers
{
    public class ObservationController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public ObservationController(
            ICommandProcessor commandProcessor)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");

            _commandProcessor = commandProcessor;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

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

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(ObservationCreateInput observationCreateInput)
        {
            _commandProcessor.Process(MakeObservationCreateCommand(observationCreateInput));

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ObservationCreateInput observationCreateInput)
        {
            _commandProcessor.Process(MakeObservationCreateCommand(observationCreateInput));

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        private ObservationCreateCommand MakeObservationCreateCommand(ObservationCreateInput observationCreateInput)
        {
            return new ObservationCreateCommand()
            {
                Title = observationCreateInput.Title,
                Latitude = observationCreateInput.Latitude,
                Longitude = observationCreateInput.Longitude,
                Address = observationCreateInput.Address,
                Username = "frankr"
            };
        }

        #endregion      

    }
}
