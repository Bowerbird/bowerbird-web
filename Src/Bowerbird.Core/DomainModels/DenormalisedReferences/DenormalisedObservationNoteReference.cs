/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using System;

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public class DenormalisedObservationNoteReference
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; private set; }

        public string UserId { get; private set; }

        public DateTime CreatedOn { get; private set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedObservationNoteReference(ObservationNote observationNote)
        {
            Check.RequireNotNull(observationNote, "observationNote");

            return new DenormalisedObservationNoteReference
            {
                Id = observationNote.Id,
                UserId = observationNote.User.Id,
                CreatedOn = observationNote.CreatedOn
            };
        }

        #endregion
    }
}