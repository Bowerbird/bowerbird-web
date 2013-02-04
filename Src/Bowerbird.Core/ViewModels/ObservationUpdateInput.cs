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

        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter the observed on date and time")]
        public DateTime ObservedOn { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }

        public string Address { get; set; }

        [Required]
        public bool AnonymiseLocation { get; set; }

        [Required(ErrorMessage = "Please select an observation category")]
        public string Category { get; set; }

        public List<ObservationMediaItem> Media { get; set; }

        public List<string> ProjectIds { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}