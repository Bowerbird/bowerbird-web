using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
