
namespace Bowerbird.Core.Tasks
{
    public interface IUserTasks : ITasks
    {
        bool AreCredentialsValid(string email, string password);

        string GetEmailByResetPasswordKey(string resetPasswordKey);

        string GetUserIdByResetPasswordKey(string resetPasswordKey);

        string GetUserIdByEmail(string email);
    }
}
