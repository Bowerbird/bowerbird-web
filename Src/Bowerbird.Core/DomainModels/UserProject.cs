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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.DomainModels
{
    public class UserProject : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected UserProject()
            : base()
        {
        }

        public UserProject(
            User createdByUser)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            User = createdByUser;

            SetDetails(
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null);

            EventProcessor.Raise(new DomainModelCreatedEvent<UserProject>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}