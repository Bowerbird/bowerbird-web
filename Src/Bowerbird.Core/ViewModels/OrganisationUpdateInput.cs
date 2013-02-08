/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.Internationalisation;
using Bowerbird.Core.Validators;

namespace Bowerbird.Core.ViewModels
{
    public class OrganisationUpdateInput
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "DescriptionRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Description { get; set; }

        public string Website { get; set; }

        public string AvatarId { get; set; }

        public string BackgroundId { get; set; }

        [EnumerableLength(1, ErrorMessageResourceName = "CategoriesRequired", ErrorMessageResourceType = typeof(I18n))]
        public IEnumerable<string> Categories { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}