/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.Internationalisation;
using Bowerbird.Core.Validators;

namespace Bowerbird.Core.ViewModels
{
    public class ObservationUpdateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Key { get; set; }

        [Required(ErrorMessageResourceName = "TitleRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "ObservedOnRequired", ErrorMessageResourceType = typeof(I18n))]
        public DateTime ObservedOn { get; set; }

        [GeoCoordinatesRequired(ErrorMessageResourceName = "LatLongRequired", ErrorMessageResourceType = typeof(I18n))]
        [GeoCoordinates(ErrorMessageResourceName = "LatLongInvalid", ErrorMessageResourceType = typeof(I18n))]
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Address { get; set; }

        public bool AnonymiseLocation { get; set; }

        [Required(ErrorMessageResourceName = "CategoryRequired", ErrorMessageResourceType = typeof(I18n))]
        public string Category { get; set; }

        [EnumerableLength(1, ErrorMessageResourceName = "MediaRequired", ErrorMessageResourceType = typeof(I18n))]
        public IEnumerable<ObservationMediaUpdateInput> Media { get; set; }

        public IEnumerable<string> ProjectIds { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}