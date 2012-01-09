using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Controllers
{
    public class HomeController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IViewModelRepository _viewModelRepository;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IViewModelRepository viewModelRepository)
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

        public ActionResult Index(HomeIndexInput homeIndexInput)
        {
            // HACK
            homeIndexInput.UserId = "frankr";
            homeIndexInput.Page = 1;
            homeIndexInput.PageSize = 10;

            return View(_viewModelRepository.Load<HomeIndexInput, HomeIndex>(homeIndexInput));
        }

        [Transaction]
        public ActionResult GenerateData()
        {
            // Create basic data for system functionality
            var setupSystemCommand = new SetupSystemCommand();
            _commandProcessor.Process(setupSystemCommand);

            _commandProcessor.Process(new ObservationCreateCommand()
            {
                Title = "test",
                Address = "aaa",
                UserId = "frankr",
                ObservedOn = DateTime.Now,
                ObservationCategory = "sdsd",
                MediaResources = new List<string>()
            });

            _commandProcessor.Process(new ObservationCreateCommand()
            {
                Title = "test",
                Address = "aaa",
                UserId = "frankr",
                ObservedOn = DateTime.Now,
                ObservationCategory = "sdsd",
                MediaResources = new List<string>()
            });

            _commandProcessor.Process(new ObservationCreateCommand()
            {
                Title = "test",
                Address = "aaa",
                UserId = "frankr",
                ObservedOn = DateTime.Now,
                ObservationCategory = "sdsd",
                MediaResources = new List<string>()
            });

            return RedirectToAction("index");
        }

        #endregion      

    }
}
