/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class TeamDeleteCommand : CommandBase
    {
        #region Members

        #endregion

        #region Constructors

        public TeamDeleteCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string UserId { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        private void InitMembers()
        {
        }

        #endregion

    }
}