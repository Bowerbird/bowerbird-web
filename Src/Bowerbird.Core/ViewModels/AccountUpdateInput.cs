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
using Bowerbird.Core.Internationalisation;
using Bowerbird.Core.Validators;
using DataAnnotationsExtensions;

namespace Bowerbird.Core.ViewModels
{
    public class AccountUpdateInput
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
        [UniqueEmail(IgnoreAuthenticatedUserEmail = true, ErrorMessageResourceName = "EmailDuplicate", ErrorMessageResourceType = typeof(I18n))]
        public string Email { get; set; }

        public string Description { get; set; }

        public string AvatarId { get; set; }

        public string BackgroundId { get; set; }

        [Required(ErrorMessageResourceName = "TimezoneRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Timezone { get; set; }

        [Required(ErrorMessageResourceName = "DefaultLicenceRequired", ErrorMessageResourceType = typeof(I18n))]
        public string DefaultLicence { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}