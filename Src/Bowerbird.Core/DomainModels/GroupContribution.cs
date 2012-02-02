using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class GroupContribution : DomainModel
    {

        #region Members

        #endregion

        #region Constructors

        protected GroupContribution()
        {
        }

        public GroupContribution(
            Group group,
            string contributionId,
            User createdByUser,
            DateTime createdDateTime)
        {
            
        }

        #endregion

        #region Properties

        public DenormalisedNamedDomainModelReference<Group> Group { get; private set; }

        public string ContributionId { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
