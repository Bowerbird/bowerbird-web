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

    public class ObservationCommentCreateCommand : CommandBase
    {
        #region Members

        #endregion

        #region Constructors

        public ObservationCommentCreateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public DateTime CommentedOn { get; set; }

        public string Comment { get; set; }

        public string ObservationId { get; set; }

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