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

namespace Bowerbird.Core.Commands
{
    public class UserSessionUpdateCommand : ICommand
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string ConnectionId { get; set; }

        public DateTime LatestHeartbeat { get; set; }

        public DateTime LatestActivity { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}