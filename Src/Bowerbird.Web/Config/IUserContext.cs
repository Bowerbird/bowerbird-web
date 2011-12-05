using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Web.Config
{
    public interface IUserContext
    {

        bool IsUserAuthenticated();

        string GetAuthenticatedUsername();

        bool HasUsernameCookieValue();

        string GetUsernameCookieValue();

        void SignUserIn(string username, bool keepUserLoggedIn);

        void SignUserOut();

    }
}
