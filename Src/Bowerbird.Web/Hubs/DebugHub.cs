/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Microsoft.AspNet.SignalR;

namespace Bowerbird.Web.Hubs
{
    public class DebugHub : Hub
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void RegisterWithDebugger()
        {
            Groups.Add(Context.ConnectionId, "debugger");

            Clients.Caller.debugToClient("Connected to debug hub with: " + Context.ConnectionId);
        }
        
        #endregion

    }
}