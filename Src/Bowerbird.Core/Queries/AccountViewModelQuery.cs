/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.ViewModels;
using Raven.Client;

namespace Bowerbird.Core.Queries
{
    public class AccountViewModelQuery : IAccountViewModelQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserContext _userContext;
        
        #endregion

        #region Constructors

        public AccountViewModelQuery(
            IDocumentSession documentSession,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeAccountLogin(string returnUrl)
        {
            return new
            {
                Email = _userContext.HasEmailCookieValue() ? _userContext.GetEmailCookieValue() : string.Empty,
                ReturnUrl = returnUrl
            };
        }

        public object MakeAccountLogin(AccountLoginInput accountLoginInput)
        {
            return new
            {
                accountLoginInput.Email,
                accountLoginInput.RememberMe,
                accountLoginInput.ReturnUrl
            };
        }

        public object MakeAccountRegister(AccountRegisterInput accountRegisterInput)
        {
            return new
            {
                accountRegisterInput.Name,
                accountRegisterInput.Email
            };
        }

        public object MakeAccountRequestPasswordReset(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            return new
            {
                accountRequestPasswordResetInput.Email
            };
        }

        public object MakeAccountResetPassword(AccountResetPasswordInput accountResetPasswordInput)
        {
            return new
            {
                accountResetPasswordInput.ResetPasswordKey
            };
        }

        #endregion
    }
}