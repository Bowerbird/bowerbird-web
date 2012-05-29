using System;
namespace Bowerbird.Core.Config
{
    public interface ISystemStateManager
    {
        void SetupSystem(bool setupTestData);
        void SwitchServicesOff();
        void SwitchServicesOn();
        void SwitchServices(bool? enableEvents = null, bool? enableEmails = null, bool? enableCommands = null);
    }
}
