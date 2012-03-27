using System;
namespace Bowerbird.Core.Config
{
    public interface ISystemStateManager
    {
        bool SystemDataSetup { get; }
        bool ExecuteCommands { get; }
        bool FireEvents { get; }
        bool SendEmails { get; }

        void DisableAllServices();
        void DisableCommandProcessor();
        void DisableEmailService();
        void DisableEventProcessor();
        void EnableAllServices();
        void EnableCommandProcessor();
        void EnableEmailService();
        void EnableEventProcessor();
        void SystemDataSetupDate(DateTime dateTime);
    }
}
