using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public interface IVotable
    {

        IEnumerable<Vote> Votes { get; }

        Vote UpdateVote(int score, DateTime createdOn, User createdByUser, string subContributionId = null);

    }
}
