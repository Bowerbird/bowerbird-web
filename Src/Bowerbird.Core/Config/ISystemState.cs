using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Config
{
    public interface ISystemState
    {
        string Id { get; }

        DateTime? CoreDataSetupDate { get; }

        bool FireEvents { get; }

        bool SendEmails { get; }

        bool ExecuteCommands { get; }

        ISystemState SetCoreDataSetupDate(DateTime dateTime);

        ISystemState TurnEventsOn();

        ISystemState TurnEventsOff();
    }
}
