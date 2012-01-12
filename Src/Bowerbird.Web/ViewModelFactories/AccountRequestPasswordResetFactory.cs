using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class AccountRequestPasswordResetFactory : ViewModelFactoryBase, IViewModelFactory<AccountRequestPasswordResetInput, AccountRequestPasswordReset>, IViewModelFactory<AccountRequestPasswordReset>
    {

        #region Members

        #endregion

        #region Constructors

        public AccountRequestPasswordResetFactory(
            IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public AccountRequestPasswordReset Make()
        {
            return new AccountRequestPasswordReset();
        }

        public AccountRequestPasswordReset Make(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            Check.RequireNotNull(accountRequestPasswordResetInput, "accountRequestPasswordResetInput");

            return new AccountRequestPasswordReset()
            {
                Email = accountRequestPasswordResetInput.Email
            };
        }

        #endregion

    }
}
