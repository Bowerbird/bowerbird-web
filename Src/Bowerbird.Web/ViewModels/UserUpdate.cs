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
using DataAnnotationsExtensions;

namespace Bowerbird.Web.ViewModels
{
    public class UserUpdate
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "First Name must be provided")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "First Name must be provided")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email must be provided")]
        [Email(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        public string Description { get; set; }

        public Avatar Avatar { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}
