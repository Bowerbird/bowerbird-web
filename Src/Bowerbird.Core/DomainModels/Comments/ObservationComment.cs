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

namespace Bowerbird.Core.DomainModels.Comments
{
    #region Namespaces

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Events;
    using System;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;

    #endregion

    public class ObservationComment : Comment
    {
        #region Members

        #endregion

        #region Constructors

        protected ObservationComment() : base()
        {
        }

        public ObservationComment(
            User createdByUser
            ,Observation observation
            ,DateTime timestamp
            ,string comment
            )
            : base(
            createdByUser,
            timestamp,
            comment
            )
        {
            Check.RequireNotNull(observation, "observation");

            SetDetails(
                observation
                );

            EventProcessor.Raise(new DomainModelCreatedEvent<ObservationComment>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedObservationReference Observation { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(Observation observation)
        {
            Observation = observation;
        }

        //public ObservationComment UpdateDetails(
        //    User updatedByUser
        //    )
        //{
        //    Check.RequireNotNull(updatedByUser, "updatedByUser");

        //    SetDetails(
        //        );

        //    EventProcessor.Raise(new DomainModelUpdatedEvent<ObservationComment>(this, updatedByUser));

        //    return this;
        //}

        #endregion
    }
}