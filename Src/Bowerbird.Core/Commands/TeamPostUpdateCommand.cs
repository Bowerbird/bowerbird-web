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

namespace Bowerbird.Core.Commands
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    #endregion

    public class TeamPostUpdateCommand : CommandBase
    {
        #region Members

        #endregion

        #region Constructors

        public TeamPostUpdateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string UserId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public IList<string> MediaResources { get; set; }

        public DateTime Timestamp { get; set; }

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