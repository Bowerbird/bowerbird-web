using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using System.Dynamic;
using Bowerbird.Core.Events;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModelFactories
{
    public class ActivityViewFactory : IActivityViewFactory
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public Activity MakeActivity(
            IDomainEvent domainEvent,
            string type,
            DateTime created,
            string description,
            IEnumerable<dynamic> groups,
            string contributionId = (string)null,
            string subContributionId = (string)null)
        {
            return new Activity(
                type,
                created,
                description,
                new
                {
                    domainEvent.User.Id,
                    domainEvent.User.Name,
                    domainEvent.User.Avatar
                },
                groups,
                contributionId,
                subContributionId);
        }

        #endregion   
    }
}
