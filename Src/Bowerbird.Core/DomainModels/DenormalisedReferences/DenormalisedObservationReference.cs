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

using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public class DenormalisedObservationReference
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Title { get; set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedObservationReference(Observation observation)
        {
            Check.RequireNotNull(observation, "observation");

            return new DenormalisedObservationReference
            {
                Id = observation.Id,
                Title = observation.Title
            };
        }

        #endregion

    }
}