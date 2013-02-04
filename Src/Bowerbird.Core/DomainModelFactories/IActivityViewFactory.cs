using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModelFactories
{
    public interface IActivityViewFactory
    {
        Activity MakeActivity(
            IDomainEvent domainEvent,
            string type,
            DateTime created,
            string description,
            IEnumerable<dynamic> groups,
            string contributionId = (string) null,
            string subContributionId = (string) null);
    }
}
