using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using System.Diagnostics;
using Ninject.Activation;
using Bowerbird.Core.CommandHandlers;

namespace Bowerbird.Web.Config
{
    public class CommandHandlerProvider : IProvider
    {
        public virtual Type Type
        {
            get { return typeof(ICommandHandler<>); }
        }

        public object Create(IContext context)
        {
            IContext x = context;

            return null;
        }
    }
}
