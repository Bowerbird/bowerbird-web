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

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Ninject.Activation;
using SignalR.Hubs;
using System.Linq;
using Bowerbird.Web.Hubs;
using SignalR;
using System;

namespace Bowerbird.Web.Config
{
    public class NinjectHubContextProvider : Provider<IHubContext>
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override IHubContext CreateInstance(IContext ctx)
        {
            var attribute = ctx.Request.Target.GetCustomAttributes(typeof(HubContextAttribute), false).Cast<HubContextAttribute>().FirstOrDefault();

            if (attribute != null)
            {
                return GlobalHost.ConnectionManager.GetHubContext(attribute.HubType.Name);
            }

            throw new Exception(string.Format("A HubContextAttribute has not been configured on the property {0}.", ctx.Request.Target.Name));
        }

        #endregion      

    }
}

