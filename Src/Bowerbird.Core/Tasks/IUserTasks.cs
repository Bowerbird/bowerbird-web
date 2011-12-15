
namespace Bowerbird.Core.Tasks
{
    public interface IUserTasks : ITasks
    {
        bool AreCredentialsValid(string identifier, string password);

        //bool IsUsernameAvailable(string username);

        bool IsEmailAvailable(string email);
    }
}
