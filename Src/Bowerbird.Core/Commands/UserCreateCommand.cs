﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;

namespace Bowerbird.Core.Commands
{
    public class UserCreateCommand : ICommand
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Timezone { get; set; }

        public IEnumerable<string> Roles { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}