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

using Bowerbird.Core.Config;
using System;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModels
{
    public class AppRoot : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected AppRoot() : base() 
        {
            EnableEvents();
        }

        /// <summary>
        /// Special constructor for setup of inital app root object in RavenDB during system setup
        /// </summary>
        internal AppRoot(
            DateTime systemSetupDate,
            IEnumerable<string> categories)
            : base()
        {
            Check.RequireNotNull(categories, "categories");

            Id = Constants.AppRootId;
            SystemSetupDate = systemSetupDate;
            Categories = categories;
            base.Name = "Bowerbird";
            base.CreatedDateTime = DateTime.UtcNow;

            EnableEvents();
        }

        #endregion

        #region Properties

        public override string GroupType
        {
            get { return "approot"; }
        }

        public IEnumerable<string> Categories { get; private set; }

        public DateTime SystemSetupDate { get; private set; }

        public bool EmailServiceStatus { get; private set; }

        public bool BackChannelServiceStatus { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            EmailServiceStatus = false;
            BackChannelServiceStatus = false;
        }

        // Special method for setup of inital app root object in RavenDB during system setup
        internal AppRoot SetCreatedByUser(User createdByUser)
        {
            User = createdByUser;
            return this;
        }

        public AppRoot SetEmailServiceStatus(bool emailServiceStatus)
        {
            EmailServiceStatus = emailServiceStatus;
            return this;
        }

        public AppRoot SetBackChannelServiceStatus(bool backChannelServiceStatus)
        {
            BackChannelServiceStatus = backChannelServiceStatus;
            return this;
        }

        #endregion
    }
}