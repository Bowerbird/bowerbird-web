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
    public class ObservationNoteCreateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string ObservationId { get; set; }

        public DateTime NotedOn { get; set; }

        public string CommonName { get; set; }

        public string ScientificName { get; set; }

        public string Taxonomy { get; set; }

        public Dictionary<string,string> Descriptions { get; set; }

        public Dictionary<string, string> References { get; set; }

        public IEnumerable<string> Tags { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}