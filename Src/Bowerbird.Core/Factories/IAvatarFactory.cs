/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.Factories
{
    public interface IAvatarFactory
    {
        object MakeDefaultAvatar(AvatarDefaultType avatarType, string altTag);
        object Make(Team team);
        object Make(Project project);
        object Make(Organisation organisation);
        object Make(User user);
    }
}