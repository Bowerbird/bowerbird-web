using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Factories
{
    public interface IAvatarFactory
    {
        Avatar GetAvatar(Team team);
        Avatar GetAvatar(Project project);
        Avatar GetAvatar(Organisation organisation);
        Avatar GetAvatar(User user);
    }
}