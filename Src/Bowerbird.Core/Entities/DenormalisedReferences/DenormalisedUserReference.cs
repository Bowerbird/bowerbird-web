using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Entities.DenormalisedReferences
{
    public class DenormalisedUserReference
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedUserReference(User user)
        {
            return new DenormalisedUserReference
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        #endregion

    }
}
