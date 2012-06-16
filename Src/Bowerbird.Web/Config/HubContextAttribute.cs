﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System;

namespace Bowerbird.Web.Config
{
    /// <summary>
    /// Sets the IHubContext parameter to a specfic SignalR hub to enable runtime loading of specified Hub.
    /// This attribute is used by Bowerbird.Web.Config.NinjectHubContextProvider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class HubContextAttribute : Attribute
    {
        private readonly Type _hubType;

        public HubContextAttribute(Type hubType)
        {
            _hubType = hubType;
        }

        public Type HubType { get { return _hubType; } }
    }
}