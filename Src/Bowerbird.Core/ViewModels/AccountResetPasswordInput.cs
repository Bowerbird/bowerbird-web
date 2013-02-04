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

using Bowerbird.Core.Validators;

namespace Bowerbird.Core.ViewModels
{
    public class AccountResetPasswordInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [ValidResetPasswordKeyRequest(ErrorMessage = "The password reset request is not valid")]
        public string ResetPasswordKey { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}