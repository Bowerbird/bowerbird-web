using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Web.Config
{
    public interface IUserContext
    {

        bool IsUserAuthenticated();

        string GetAuthenticatedUserId();

        bool HasEmailCookieValue();

        string GetEmailCookieValue();

        void SignUserIn(string email, bool keepUserLoggedIn);

        void SignUserOut();

        dynamic GetChannel();

        bool HasGlobalPermission(string permissionId);

        bool HasTeamPermission(string teamId, string permissionId);

        bool HasProjectPermission(string projectId, string permissionId);

        bool HasPermissionToUpdate<T>(string id);

        bool HasPermissionToDelete<T>(string id);

    }
}
