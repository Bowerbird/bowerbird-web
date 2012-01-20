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

using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Web.ViewModels.Members
{
    public class AccountChangePasswordInput
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "Please enter a password")]
        [StringLength(1000, MinimumLength = 6, ErrorMessage = "Passwords must be at least 6 characters in length")]
        public string Password { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
