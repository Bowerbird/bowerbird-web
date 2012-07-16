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
using System.Linq;
using System.Web;
using System.Web.Security;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Repositories;
using Raven.Client;
using SignalR;
using SignalR.Hubs;
using SignalR.Infrastructure;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.DomainModels;

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

        //public void SendModelToUserClient(object model, string clientId)
        //{
        //    _userHub.Clients[clientId].asynchModelCreated(model);
        //}

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

        public bool HasAppRootPermission(string permissionId)
        {
            return _permissionChecker.HasGroupPermission(permissionId, GetAuthenticatedUserId(), Constants.AppRootId);
        }

        public bool HasUserPermission(string domainModelId)
        {
            var user = _documentSession.Load<User>(domainModelId);

            return user != null && user.Id == GetAuthenticatedUserId();
        }

        public bool HasUserProjectPermission(string permissionId) 
        {
            var userProject = _documentSession.Query<UserProject>().Where(x => x.User.Id == GetAuthenticatedUserId()).First();
            return _permissionChecker.HasGroupPermission(permissionId, GetAuthenticatedUserId(), userProject.Id);
        }

        public bool HasGroupPermission(string permissionId, string groupId)
        {
            return _permissionChecker.HasGroupPermission(permissionId, GetAuthenticatedUserId(), groupId);
        }

        public bool HasGroupPermission<T>(string permissionId, string domainModelId)
            where T : IOwnable
        {
            return _permissionChecker.HasGroupPermission<T>(permissionId, GetAuthenticatedUserId(), domainModelId);
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

        public void SendModelToUserClient(object model, string clientId)
        {
            throw new NotImplementedException();
        }

        public string GetUserFullName()
        {
            return HttpContext.Current.Request.LogonUserIdentity != null ? HttpContext.Current.Request.LogonUserIdentity.Name : string.Empty;
        }

        #endregion

    }
}