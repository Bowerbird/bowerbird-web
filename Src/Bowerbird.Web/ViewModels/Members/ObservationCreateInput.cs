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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Web.Validators;

namespace Bowerbird.Web.ViewModels.Members
{
    public class ObservationCreateInput
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter the observed on date and time")]
        public DateTime ObservedOn { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Address { get; set; }

        [Required]
        public bool IsIdentificationRequired { get; set; }

        [Required(ErrorMessage = "Please select an observation category")]
        public string ObservationCategory { get; set; }

        public List<string> MediaResources { get; set; }

        public List<string> Projects { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
