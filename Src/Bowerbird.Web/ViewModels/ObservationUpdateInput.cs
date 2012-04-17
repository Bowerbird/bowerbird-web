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

namespace Bowerbird.Web.ViewModels
{
    public class ObservationUpdateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string ObservationId { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public bool IsIdentificationRequired { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public DateTime ObservedOn { get; set; }

        public string UserId { get; set; }

        public Dictionary<string, string> AddMediaResources { get; set; }

        public Dictionary<string, string> MediaResources { get; set; }

        public List<string> RemoveMediaResources { get; set; }

        public List<string> Projects { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}