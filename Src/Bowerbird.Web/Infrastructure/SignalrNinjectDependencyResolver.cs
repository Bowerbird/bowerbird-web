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
using Bowerbird.Core.DesignByContract;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Json;
using Ninject;
//using SignalR;

namespace Bowerbird.Web.Infrastructure
{
    public class SignalrNinjectDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;

        public SignalrNinjectDependencyResolver(IKernel kernel)
        {
            Check.RequireNotNull(kernel, "kernel");

            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            if (typeof(IConnection).Assembly == serviceType.Assembly && serviceType != typeof(IJsonSerializer)) // Push DI for SignalR types to base
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
            if (typeof(IConnection).Assembly == serviceType.Assembly && serviceType != typeof(IJsonSerializer)) // Push DI for SignalR types to base
            {
                return base.GetServices(serviceType);
            }
            else
            {
                return _kernel.GetAll(serviceType);
            }
        }
    }
}