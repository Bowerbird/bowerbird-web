 /* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Web.ViewModels
{
    public class CommentCreateInput
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public bool IsNested { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string ContributionId { get; set; }

        public string ParentCommentId { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}