/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Core.Commands;
using Bowerbird.Web.ViewModels.Members;

namespace Bowerbird.Web.Controllers.Members
{
    public class AccountController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public AccountController(
            ICommandProcessor commandProcessor,
            IUserContext userContext)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods



        #endregion

    }
}
