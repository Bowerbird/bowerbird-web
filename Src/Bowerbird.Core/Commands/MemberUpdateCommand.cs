﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
    public class MemberUpdateCommand : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string GroupId { get; set; }

        public IEnumerable<string> AddRoleIds { get; set; }

        public IEnumerable<string> RemoveRoleIds { get; set; }

        public string ModifiedByUserId { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}