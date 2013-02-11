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
using Bowerbird.Core.Internationalisation;
using Bowerbird.Core.Validators;

namespace Bowerbird.Core.ViewModels
{
    public class SightingNoteUpdateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Used when adding an note at the same time as a new sighting only.
        /// </summary>
        public bool NewSightingNote { get; set; }

        public int? Id { get; set; }

        public string SightingId { get; set; }

        [DescriptionOrTagRequired(ErrorMessageResourceName = "DescriptionOrTagRequired", ErrorMessageResourceType = typeof(I18n))]
        public Dictionary<string, string> Descriptions { get; set; }

        public string Tags { get; set; }

        public string NoteComments { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}