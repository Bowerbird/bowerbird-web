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
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Commands
{
    public class NotificationCreatedCommand : ICommand
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public DateTime Timestamp { get; set; }

        public Activity Activity { get; set; }

        public IEnumerable<string> UserIds { get; set; }//ken walker created the 'bees in carlton' project

        #endregion

        #region Methods

        #endregion
    }
}