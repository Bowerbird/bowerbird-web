
namespace Bowerbird.Core.Tasks
{
    public interface IUserTasks : ITasks
    {
        bool AreCredentialsValid(string email, string password);

        bool EmailExists(string email);
    }
}
