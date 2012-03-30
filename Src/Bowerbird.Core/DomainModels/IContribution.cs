using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public interface IContribution
    {
        string Id { get; }

        DenormalisedUserReference User { get; }

        DateTime CreatedOn { get; }
    }
}
