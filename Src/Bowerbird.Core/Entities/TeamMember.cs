using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;
using System;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Entities
{
    public class TeamMember : Member
    {

        #region Members

        #endregion

        #region Constructors

        protected TeamMember()
            : base()
        {
            InitMembers();
        }

        public TeamMember(
            User createdByUser,
            Team team,
            User user,
            IEnumerable<Role> roles)
            : base(
            user,
            roles)
        {
            Check.RequireNotNull(team, "team");

            EventProcessor.Raise(new EntityCreatedEvent<TeamMember>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedNamedEntityReference<Team> Team { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        private void SetDetails(Team team, User user)
        {
            Team = team;

            base.SetDetails(user);
        }

        #endregion

    }
}