using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class GroupAssociation : ValueObject
    {

        #region Members

        #endregion

        #region Constructors

        public GroupAssociation(
            Group parentGroup,
            Group childGroup,
            User createdByUser,
            DateTime createdDateTime)
            : base()
        {
            Check.RequireNotNull(parentGroup, "parentGroup");
            Check.RequireNotNull(childGroup, "childGroup");
            Check.RequireNotNull(createdByUser, "createdByUser");

            ParentGroup = parentGroup;
            ChildGroup = childGroup;
            CreatedByUser = createdByUser;
            CreatedDateTime = createdDateTime;

            EventProcessor.Raise(new DomainModelCreatedEvent<GroupAssociation>(this, createdByUser));
        }

        #endregion

        #region Properties

        public Group ParentGroup { get; private set; }

        public Group ChildGroup { get; private set; }

        public User CreatedByUser { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
