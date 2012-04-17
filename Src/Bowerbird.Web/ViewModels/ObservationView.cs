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
using System;

namespace Bowerbird.Web.ViewModels
{
    public class ObservationView : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime ObservedOn { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Address { get; set; }

        public bool IsIdentificationRequired { get; set; }

        public string ObservationCategory { get; set; }

        public IEnumerable<ObservationMediaItem> ObservationMedia { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}