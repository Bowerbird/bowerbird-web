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

namespace Bowerbird.Core.Commands
{
    public class OrganisationPostCreateCommand : ICommand
    {
        #region Fields

        #endregion

        #region Constructors

        public OrganisationPostCreateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string OrganisationId { get; set; }

        public string Message { get; set; }

        public string Subject { get; set; }

        public DateTime PostedOn { get; set; }

        public IList<string> MediaResources { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            MediaResources = new List<string>();
        }

        #endregion
    }
}