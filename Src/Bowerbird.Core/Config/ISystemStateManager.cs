using System;
namespace Bowerbird.Core.Config
{
    public interface ISystemStateManager
    {
        void SetupSystem(bool setupTestData);
        void DisableAllServices();
        void DisableCommandProcessor();
        void DisableEmailService();
        void DisableEventProcessor();
        void EnableAllServices();
        void EnableCommandProcessor();
        void EnableEmailService();
        void EnableEventProcessor();
    }
}
