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
using Bowerbird.Core.Events;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels.Comments
{
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
            ,DateTime commentedOn
            ,string comment
            )
            : base(
            createdByUser,
            commentedOn,
            comment
            )
        {
            Check.RequireNotNull(observation, "observation");

            Observation = observation;

            EventProcessor.Raise(new DomainModelCreatedEvent<ObservationComment>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedObservationReference Observation { get; private set; }

        #endregion

        #region Methods

        public ObservationComment UpdateCommentMessage(
            User updatedByUser,
            DateTime updatedOn,
            string message
            )
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            UpdateDetails(
                updatedByUser,
                updatedOn,
                message
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<ObservationComment>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}