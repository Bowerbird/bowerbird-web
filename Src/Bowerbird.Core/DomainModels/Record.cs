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

using System;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Record : Sighting
    {
        #region Members

        #endregion

        #region Constructors

        protected Record()
            : base()
        {
            EnableEvents();
        }

        public Record(
            User createdByUser,
            DateTime createdOn,
            DateTime observedOn,
            string latitude, 
            string longitude, 
            bool anonymiseLocation,
            string category,
            UserProject userProject,
            IEnumerable<Project> projects
            ) 
            : base(
            createdByUser,
            createdOn,
            observedOn,
            latitude,
            longitude,
            anonymiseLocation,
            category,
            userProject,
            projects
            )
        {
            EnableEvents();

            FireEvent(new DomainModelCreatedEvent<Record>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public Record UpdateDetails(User updatedByUser,
            DateTime observedOn,
            string latitude,
            string longitude,
            bool anonymiseLocation,
            string category)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            base.SetDetails(
                observedOn,
                latitude,
                longitude,
                anonymiseLocation,
                category);

            FireEvent(new DomainModelUpdatedEvent<Record>(this, updatedByUser, this));

            return this;
        }

        #endregion

    }
}