using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using Bowerbird.Web.Config;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;

namespace Bowerbird.Web.Config
{
    public class UserContext : IUserContext
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool IsUserAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public string GetAuthenticatedUsername()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public bool HasUsernameCookieValue()
        {
            return GetCookie(Constants.UsernameCookieName) != null;
        }

        public string GetUsernameCookieValue()
        {
            return GetCookie(Constants.UsernameCookieName).Value;
        }

        public void SignUserIn(string username, bool keepUserLoggedIn)
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

            var authTicket = new FormsAuthenticationTicket(username, keepUserLoggedIn, Convert.ToInt32(sessionExpiryDuration.TotalMinutes)); // Must be less than cookie expiration, whic we have set to 100 years

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            // Add the forms auth session cookie to log user in
            AddCookie(FormsAuthentication.FormsCookieName, encryptedTicket, string.Empty);

            // Add the username into cookie for reference on next login
            AddCookie(Constants.UsernameCookieName, username, string.Empty);
        }

        public void SignUserOut()
        {
            FormsAuthentication.SignOut();
        }

        public dynamic GetChannel()
        {
            return Hub.GetClients<ActivityHub>();
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
