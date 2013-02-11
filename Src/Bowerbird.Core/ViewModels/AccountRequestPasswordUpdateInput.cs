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
    public class AccountRequestPasswordUpdateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(I18n))]
        [Email(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(I18n))]
        [ValidEmail(ErrorMessageResourceName = "EmailDoesNotExist", ErrorMessageResourceType = typeof(I18n))]
        public string Email { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}