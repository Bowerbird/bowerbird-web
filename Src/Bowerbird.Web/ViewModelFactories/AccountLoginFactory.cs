using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModels.Public;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class AccountLoginFactory : ViewModelFactoryBase, IViewModelFactory<AccountLoginInput, AccountLogin>, IViewModelFactory<AccountLogin>
    {

        #region Members

        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public AccountLoginFactory(
            IUserContext userContext,
            IDocumentSession documentSession)
            : base(documentSession)
        {
            Check.RequireNotNull(userContext, "userContext");

            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public AccountLogin Make()
        {
            return new AccountLogin()
                       {
                           Email = _userContext.HasEmailCookieValue() ? _userContext.GetEmailCookieValue() : string.Empty
                       };
        }

        public AccountLogin Make(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            return new AccountLogin()
            {
                Email = accountLoginInput.Email,
                RememberMe = accountLoginInput.RememberMe,
                ReturnUrl = accountLoginInput.ReturnUrl
            };
        }

        #endregion      
      
    }
}
