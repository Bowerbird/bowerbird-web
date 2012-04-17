/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;

namespace Bowerbird.Web.ViewModels
{
    public class Notification
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public DateTime OccurredOn { get; set; }

        public string UserId { get; set; }

        public string Action { get; set; }

        public string AvatarUri { get; set; }

        public string SummaryDescription { get; set; }

        public string CreatedDateTimeDescription { get; set; }

        public object Model { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}