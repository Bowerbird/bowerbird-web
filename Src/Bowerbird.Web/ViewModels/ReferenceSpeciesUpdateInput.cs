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
    public class ReferenceSpeciesUpdateInput
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string SpeciesId { get; set; }

        public string GroupId { get; set; }

        public string UserId { get; set; }

        public IEnumerable<string> SmartTags { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}