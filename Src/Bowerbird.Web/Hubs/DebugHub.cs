/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Threading.Tasks;
using SignalR.Hubs;

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

            Caller.debugToClient("Connected to debug hub with: " + Context.ConnectionId);
        }

        //public Task Disconnect()
        //{
        //    return Groups.Remove(Context.ConnectionId, "debugger");
        //}

        #endregion

    }
}