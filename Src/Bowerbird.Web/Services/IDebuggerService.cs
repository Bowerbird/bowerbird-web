
using System;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Services
{
    public interface IDebuggerService : IService
    {
        void DebugToClient(object output);
    }
}