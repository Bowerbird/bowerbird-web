using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Controllers
{
    public class UserController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;


        #endregion

        #region Constructors

        public UserController(
            ICommandProcessor commandProcessor)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");

            _commandProcessor = commandProcessor;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [Transaction]
        public ActionResult Create(UserCreateInput userCreateInput)
        {
            _commandProcessor.Process(MakeUserCreateCommand(userCreateInput));

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        private UserCreateCommand MakeUserCreateCommand(UserCreateInput userCreateInput)
        {
            return new UserCreateCommand()
            {
                Username = userCreateInput.Username
            };
        }

        #endregion

    }
}
