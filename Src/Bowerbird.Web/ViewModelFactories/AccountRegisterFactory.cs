using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class AccountRegisterFactory : ViewModelFactoryBase, IViewModelFactory<AccountRegisterInput, AccountRegister>, IViewModelFactory<AccountRegister>
    {

        #region Members

        #endregion

        #region Constructors

        public AccountRegisterFactory(
            IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public AccountRegister Make()
        {
            return new AccountRegister();
        }

        public AccountRegister Make(AccountRegisterInput accountRegisterInput)
        {
            Check.RequireNotNull(accountRegisterInput, "accountRegisterInput");

            return new AccountRegister()
            {
                FirstName = accountRegisterInput.FirstName,
                LastName = accountRegisterInput.LastName,
                Email = accountRegisterInput.Email
            };
        }

        #endregion

    }
}
