using System;
namespace Bowerbird.Core.Config
{
    public interface ISystemStateManager
    {
        void SetupSystem(bool setupTestData);
        void SwitchServicesOff();
        void SwitchServicesOn();
        void SwitchServices(bool? enableEmailService = null, bool? enableChannelService = null);
    }
}
