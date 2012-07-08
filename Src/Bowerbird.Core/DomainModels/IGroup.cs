 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public interface IGroup
    {
        string Id { get; }

        string Name { get; }

        DateTime CreatedDateTime { get; }

        DenormalisedUserReference User { get; }

        IEnumerable<DenormalisedGroupReference> Ancestry { get; }

        IEnumerable<DenormalisedGroupReference> Descendants { get; }
    }
}
