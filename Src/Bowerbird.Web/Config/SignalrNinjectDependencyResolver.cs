/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

// attribution: https://raw.github.com/SignalR/SignalR.Ninject/master/SignalR.Ninject/NinjectDependencyResolver.cs				

using System;
using System.Linq;
using System.Collections.Generic;
using Ninject;
using SignalR;

namespace Bowerbird.Web.Config
{
    public class SignalrNinjectDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;

        public SignalrNinjectDependencyResolver(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            if(typeof(SignalR.IConnection).Assembly == serviceType.Assembly) // Push DI for SignalR types to base
            {
                return base.GetService(serviceType);
            }
            else
            {
                return _kernel.TryGet(serviceType);
            }
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }
}