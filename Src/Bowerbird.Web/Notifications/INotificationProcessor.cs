/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels.Shared;
using Bowerbird.Web.ViewModels;
using System;

namespace Bowerbird.Web.Notifications
{
    public interface INotificationProcessor
    {
        void Notify(Notification notification, Action<dynamic, Notification> callClient);
    }
}