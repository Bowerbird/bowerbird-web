
using System;
using Bowerbird.Core.Services;

namespace Bowerbird.Core.Services
{
    public interface IDebuggerService : IService
    {
        void DebugToClient(object output);
    }
}