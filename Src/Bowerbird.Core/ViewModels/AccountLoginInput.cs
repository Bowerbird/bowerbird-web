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
using Bowerbird.Core.Internationalisation;
using DataAnnotationsExtensions;

namespace Bowerbird.Core.ViewModels
{
    public class AccountLoginInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(I18n))]
        [Email(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(I18n))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}