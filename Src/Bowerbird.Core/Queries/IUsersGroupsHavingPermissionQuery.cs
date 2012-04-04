using System.Collections.Generic;

namespace Bowerbird.Core.Queries
{
    public interface IUsersGroupsHavingPermissionQuery
    {
        IEnumerable<string> GetUsersGroupsHavingPermission(string userId, string permissionId);
    }
}