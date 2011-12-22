using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class AccountLoginFactory : ViewModelFactoryBase<AccountLoginInput, AccountLogin>
    {

        #region Members

        #endregion

        #region Constructors

        public AccountLoginFactory(
            IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override AccountLogin Make(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            return new AccountLogin()
            {
                Username = accountLoginInput.Username,
                RememberMe = accountLoginInput.RememberMe,
                ReturnUrl = accountLoginInput.ReturnUrl
            };
        }

        #endregion      
      
    }
}
