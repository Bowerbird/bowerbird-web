using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Tasks
{
    public interface IUserTasks : ITasks
    {
        bool AreCredentialsValid(string username, string password);

        bool IsUsernameAvailable(string username);

        bool IsEmailAvailable(string email);
    }
}
