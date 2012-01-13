/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;

    #endregion

    public class ObservationUpdateInput : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        public ObservationUpdateInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string ObservationId { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public bool IsIdentificationRequired { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Title { get; set; }

        public string ObservationCategory { get; set; }

        public DateTime ObservedOn { get; set; }

        public string UserId { get; set; }

        public List<string> MediaResources { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        #endregion
    }
}