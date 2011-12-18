﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;
using System;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Entities
{
    public class GlobalMember : Member
    {

        #region Members

        #endregion

        #region Constructors

        protected GlobalMember()
            : base()
        {
            InitMembers();
        }

        public GlobalMember(
            User user,
            IEnumerable<Role> roles)
            : base(
            user,
            roles)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        private new void SetDetails(User user)
        {
            base.SetDetails(user);
        }

        #endregion

    }
}