using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class AccountResetPasswordFactory : ViewModelFactoryBase, IViewModelFactory<AccountResetPasswordInput, AccountResetPassword>, IViewModelFactory<AccountResetPassword>
    {

        #region Members

        #endregion

        #region Constructors

        public AccountResetPasswordFactory(
            IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public AccountResetPassword Make()
        {
            return new AccountResetPassword();
        }

        public AccountResetPassword Make(AccountResetPasswordInput accountResetPasswordInput)
        {
            Check.RequireNotNull(accountResetPasswordInput, "accountResetPasswordInput");

            return new AccountResetPassword()
            {
                ResetPasswordKey = accountResetPasswordInput.ResetPasswordKey
            };
        }

        #endregion

    }
}
