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
    public class ChatRequest
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Message { get; set; }

        public string ChatId { get; set; }

        public UserProfile FromUser { get; set; }

        public UserProfile ToUser { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}