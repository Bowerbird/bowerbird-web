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

namespace Bowerbird.Core.Services
{
    public interface INotificationService : IService
    {
        void SendActivity(Activity activity);

        void SendUserStatusUpdate(object userStatus);
    }
}