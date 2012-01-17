using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class UserUpdateFactory : ViewModelFactoryBase, IViewModelFactory<UserUpdateInput, UserUpdate>, IViewModelFactory<UserUpdate>
    {

        #region Members

        #endregion

        #region Constructors

        public UserUpdateFactory(
            IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public UserUpdate Make()
        {
            return new UserUpdate();
        }

        public UserUpdate Make(UserUpdateInput userUpdateInput)
        {
            Check.RequireNotNull(userUpdateInput, "userUpdateInput");

            return new UserUpdate()
            {
                FirstName = userUpdateInput.FirstName,
                LastName = userUpdateInput.LastName,
                Email = userUpdateInput.Email,
                Description = userUpdateInput.Description
            };
        }

        #endregion

    }
}
