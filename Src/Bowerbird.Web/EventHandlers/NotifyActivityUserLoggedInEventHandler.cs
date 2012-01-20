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

using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
				
namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityUserLoggedInEventHandler : NotifyActivityEventHandlerBase, IEventHandler<UserLoggedInEvent>
    {
        #region Members

        #endregion

        #region Constructors

        public NotifyActivityUserLoggedInEventHandler(
            IUserContext userContext)
            : base(userContext)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserLoggedInEvent userLoggedInEvent)
        {
            Check.RequireNotNull(userLoggedInEvent, "userLoggedInEvent");

            Notify(
                "userloggedin",
                userLoggedInEvent.User,
                userLoggedInEvent.User);
        }

        #endregion
    }
}