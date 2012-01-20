/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Web;
using System.Web.Security;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Repositories;
using Raven.Client;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;

namespace Bowerbird.Web.Config
{
    public class UserContext : IUserContext
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IPermissionChecker _permissionChecker;

        #endregion

        #region Constructors

        public UserContext(
            IDocumentSession documentSession,
            IPermissionChecker permissionChecker)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionChecker, "permissionChecker");

            _documentSession = documentSession;
            _permissionChecker = permissionChecker;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool IsUserAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public string GetAuthenticatedUserId()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public bool HasEmailCookieValue()
        {
            return GetCookie(Constants.EmailCookieName) != null;
        }

        public string GetEmailCookieValue()
        {
            return GetCookie(Constants.EmailCookieName).Value;
        }

        public void SignUserIn(string email, bool keepUserLoggedIn)
        {
            TimeSpan sessionExpiryDuration;

            if (keepUserLoggedIn)
            {
                // 2 week expiry
                sessionExpiryDuration = new TimeSpan(14, 0, 0, 0);
            }
            else
            {
                // 3 hour expiry
                sessionExpiryDuration = new TimeSpan(3, 0, 0);
            }

            var authTicket = new FormsAuthenticationTicket(_documentSession.LoadUserByEmail(email).Id, keepUserLoggedIn, Convert.ToInt32(sessionExpiryDuration.TotalMinutes)); // Must be less than cookie expiration, whic we have set to 100 years

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            // Add the forms auth session cookie to log user in
            AddCookie(FormsAuthentication.FormsCookieName, encryptedTicket, string.Empty);

            // Add the email into cookie for reference on next login
            AddCookie(Constants.EmailCookieName, email, string.Empty);
        }

        public void SignUserOut()
        {
            FormsAuthentication.SignOut();
        }

        public dynamic GetChannel()
        {
            return Hub.GetClients<ActivityHub>();
        }

        public bool HasGlobalPermission(string permissionId)
        {
            return _permissionChecker.HasGlobalPermission(GetAuthenticatedUserId(), permissionId);
        }

        public bool HasTeamPermission(string teamId, string permissionId)
        {
            return _permissionChecker.HasTeamPermission(GetAuthenticatedUserId(), teamId, permissionId);
        }

        public bool HasProjectPermission(string projectId, string permissionId)
        {
            return _permissionChecker.HasProjectPermission(GetAuthenticatedUserId(), projectId, permissionId);
        }

        public bool HasPermissionToUpdate<T>(string id)
        {
            return _permissionChecker.HasPermissionToUpdate<T>(GetAuthenticatedUserId(), id);
        }

        public bool HasPermissionToDelete<T>(string id)
        {
            return _permissionChecker.HasPermissionToDelete<T>(GetAuthenticatedUserId(), id);
        }

        private void AddCookie(string name, string value, string domain)
        {
            HttpCookie cookie = new HttpCookie(name, value)
                                    {
                                        Expires = DateTime.Today.AddYears(100)
                                    };

            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private HttpCookie GetCookie(string name)
        {
            return HttpContext.Current.Request.Cookies[name];
        }

        #endregion      
      
    }
}
