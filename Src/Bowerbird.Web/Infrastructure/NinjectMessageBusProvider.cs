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

using Bowerbird.Core.Infrastructure;
using Ninject.Activation;

namespace Bowerbird.Web.Infrastructure
{
    public class NinjectMessageBusProvider : Provider<IMessageBus>
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override IMessageBus CreateInstance(IContext ctx)
        {
            return new MessageBus(ctx.Kernel);
        }

        #endregion      

    }
}