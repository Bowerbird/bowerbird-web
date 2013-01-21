using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Vote : IContribution, ISubContribution
    {

        #region Fields

        #endregion

        #region Constructors

        protected Vote()
            : base()
        {
        }

        public Vote(
            int id,
            User createdByUser,
            int score,
            DateTime createdOn)
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            SequentialId = id;
            Id = id.ToString();
            User = createdByUser;
            CreatedOn = createdOn;
            Score = score;
        }

        #endregion

        #region Properties

        public string Id { get; private set; }

        public int SequentialId { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public int Score { get; private set; }

        #endregion

        #region Methods

        public ISubContribution GetSubContribution(string type, string id)
        {
            return null;
        }

        #endregion

    }
}
