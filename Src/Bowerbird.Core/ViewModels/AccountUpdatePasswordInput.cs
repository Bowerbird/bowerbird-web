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

namespace Bowerbird.Core.ViewModels
{
    public class AccountUpdatePasswordKeyInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [PasswordKey(ErrorMessageResourceName = "PasswordKeyInvalid", ErrorMessageResourceType = typeof(I18n))]
        public string Key { get; set; }

        #endregion

        #region Methods

        #endregion      
    }

    public class AccountUpdatePasswordInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [PasswordKey(ErrorMessageResourceName = "PasswordKeyInvalid", ErrorMessageResourceType = typeof(I18n))]
        public string Key { get; set; }

        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(I18n))]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = "PasswordTooShort", ErrorMessageResourceType = typeof(I18n))]
        public string NewPassword { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}