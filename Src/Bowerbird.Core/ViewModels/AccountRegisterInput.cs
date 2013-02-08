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
using Bowerbird.Core.Validators;
using DataAnnotationsExtensions;

namespace Bowerbird.Core.ViewModels
{
    public class AccountRegisterInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessageResourceName = "UserNameRequired", ErrorMessageResourceType = typeof(I18n))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "NameTooShort", ErrorMessageResourceType = typeof(I18n))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(I18n))]
        [Email(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(I18n))]
        [UniqueEmail(ErrorMessageResourceName = "EmailDuplicate", ErrorMessageResourceType = typeof(I18n))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(I18n))]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = "PasswordTooShort", ErrorMessageResourceType = typeof(I18n))]
        public string Password { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}