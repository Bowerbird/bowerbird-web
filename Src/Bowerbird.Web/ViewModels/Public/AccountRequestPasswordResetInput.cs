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
using Bowerbird.Web.Validators;
using DataAnnotationsExtensions;

namespace Bowerbird.Web.ViewModels.Public
{
    public class AccountRequestPasswordResetInput
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "Please enter your email address")]
        [Email(ErrorMessage = "Please enter a valid email address")]
        [ValidEmail(ErrorMessage = "The email address does not exist, please enter another email address")]
        public string Email { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
