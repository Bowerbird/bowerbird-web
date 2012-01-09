
namespace Bowerbird.Core.Tasks
{
    public interface IUserTasks : ITasks
    {
        bool AreCredentialsValid(string email, string password);

        bool IsEmailAvailable(string email);
    }
}
