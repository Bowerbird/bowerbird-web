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

        protected AppRoot() : base() { }

        // Special constructor for setup of inital app root object in RavenDB during system setup
        internal AppRoot(
            DateTime systemSetupDate,
            IEnumerable<string> categories)
            : base()
        {
            Check.RequireNotNull(categories, "categories");

            Id = Constants.AppRootId;
            SystemSetupDate = systemSetupDate;
            Categories = categories;
            base.Name = "Application Root";
            base.CreatedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        public override string GroupType
        {
            get { return "approot"; }
        }

        public IEnumerable<string> Categories { get; private set; }

        public DateTime SystemSetupDate { get; private set; }

        public bool FireEvents { get; private set; }

        public bool SendEmails { get; private set; }

        public bool ExecuteCommands { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            FireEvents = false;
            SendEmails = false;
            ExecuteCommands = false;
        }

        // Special method for setup of inital app root object in RavenDB during system setup
        internal AppRoot SetCreatedByUser(User createdByUser)
        {
            User = createdByUser;
            return this;
        }

        public AppRoot SetFireEvents(bool fireEvents)
        {
            FireEvents = fireEvents;
            return this;
        }

        public AppRoot SetSendEmails(bool sendEmails)
        {
            SendEmails = sendEmails;
            return this;
        }

        public AppRoot SetExecuteCommands(bool executeCommands)
        {
            ExecuteCommands = executeCommands;
            return this;
        }

        #endregion
    }
}