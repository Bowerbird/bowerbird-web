using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using System.Linq;

namespace Bowerbird.Web.ViewModelFactories
{
    public class UserUpdateFactory : ViewModelFactoryBase, IViewModelFactory<UserUpdateInput, UserUpdate>, IViewModelFactory<IdInput, UserUpdate>
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

        public UserUpdate Make(IdInput idInput)
        {
            var user = DocumentSession.Load<User>(idInput.Id);

            return new UserUpdate()
                       {
                           FirstName = user.FirstName,
                           LastName = user.LastName,
                           Email = user.Email,
                           Description = user.Description
                       };
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
