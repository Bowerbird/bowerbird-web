namespace Bowerbird.Web.Config
{
    public interface IPermissionChecker
    {
        /// <summary>
        /// This must be called prior to use
        /// </summary>
        void Init();

        bool HasGlobalPermission(string userId, string permissionId);

        bool HasTeamPermission(string userId, string teamId, string permissionId);

        bool HasProjectPermission(string userId, string projectId, string permissionId);

        bool HasPermissionToUpdate<T>(string userId, string id);

        bool HasPermissionToDelete<T>(string userId, string id);
    }
}