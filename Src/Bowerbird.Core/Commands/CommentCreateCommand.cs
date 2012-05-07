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
    public class CommentCreateCommand : ICommand
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string ContributionId { get; set; }

        public string UserId { get; set; }

        public DateTime CommentedOn { get; set; }

        public string Comment { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}