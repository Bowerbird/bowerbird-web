using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR.Hubs;

namespace Bowerbird.Web.Config
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class HubContextAttribute : Attribute
    {
        private readonly Type _hubType;

        public HubContextAttribute(Type hubType)
        {
            _hubType = hubType;
        }

        public Type HubType { get { return _hubType; } }
    }
}
