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
        private readonly ICommandBuilder _commandBuilder;

        #endregion

        #region Constructors

        public ObservationController(
            ICommandProcessor commandProcessor,
            ICommandBuilder commandBuilder)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(commandBuilder, "commandBuilder");

            _commandProcessor = commandProcessor;
            _commandBuilder = commandBuilder;
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
            _commandProcessor.Process(
                _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(
                    observationCreateInput, 
                    x => x.Username = User.Identity.Name));

            return Json("success"); // TODO: Return something more meaningful?
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(ObservationCreateInput observationCreateInput)
        {
            throw new NotImplementedException();
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ObservationCreateInput observationCreateInput)
        {
            throw new NotImplementedException();
        }

        #endregion      

    }
}
