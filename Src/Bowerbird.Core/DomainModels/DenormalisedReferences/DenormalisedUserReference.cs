
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public class DenormalisedUserReference
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedUserReference(User user)
        {
            Check.RequireNotNull(user, "user");

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
