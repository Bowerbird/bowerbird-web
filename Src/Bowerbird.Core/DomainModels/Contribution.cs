using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public abstract class Contribution : DomainModel
    {

        #region Members

        #endregion

        #region Constructors

        protected Contribution()
        {
        }

        protected Contribution(
            User createdByUser,
            DateTime createdOn) 
            : this() 
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            User = createdByUser;
            CreatedOn = createdOn;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
