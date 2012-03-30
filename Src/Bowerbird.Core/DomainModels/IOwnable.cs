using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public interface IOwnable
    {

        DenormalisedUserReference User { get; }

        IEnumerable<string> Groups { get; }

    }
}
