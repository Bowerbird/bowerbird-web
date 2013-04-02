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
using System.Linq;
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
            InitMembers();
        }

        /// <summary>
        /// Special constructor for setup of inital app root object in RavenDB during system setup
        /// </summary>
        internal AppRoot(
            DateTime systemSetupDate)
            : base()
        {
            InitMembers();

            Id = Constants.AppRootId;
            SystemSetupDate = systemSetupDate;
            base.Name = "Bowerbird";
            base.CreatedDateTime = DateTime.UtcNow;

            PerformSpeciesDataUpdate = true; // Set this to true so that initial species load is performed
        }

        #endregion

        #region Properties

        public override string GroupType
        {
            get { return "approot"; }
        }

        //public IEnumerable<Category> Categories { get; private set; }

        public DateTime SystemSetupDate { get; private set; }

        public bool EmailServiceStatus { get; private set; }

        public bool BackChannelServiceStatus { get; private set; }

        public bool ImageServiceStatus { get; private set; }

        public bool YouTubeVideoServiceStatus { get; private set; }

        public bool VimeoVideoServiceStatus { get; private set; }

        public bool DocumentServiceStatus { get; private set; }

        public bool AudioServiceStatus { get; private set; }

        public bool PerformSpeciesDataUpdate { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            EmailServiceStatus = false;
            BackChannelServiceStatus = false;
            ImageServiceStatus = false;
            YouTubeVideoServiceStatus = false;
            VimeoVideoServiceStatus = false;
            DocumentServiceStatus = false;
            AudioServiceStatus = false;

            PerformSpeciesDataUpdate = false;
        }

        public AppRoot SetPerformSpeciesDataUpdate(bool performSpeciesDataUpdate)
        {
            PerformSpeciesDataUpdate = performSpeciesDataUpdate;
            return this;
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

        public AppRoot SetImageServiceStatus(bool imageServiceStatus)
        {
            ImageServiceStatus = imageServiceStatus;
            return this;
        }

        public AppRoot SetYouTubeVideoServiceStatus(bool youTubeVideoServiceStatus)
        {
            YouTubeVideoServiceStatus = youTubeVideoServiceStatus;
            return this;
        }

        public AppRoot SetVimeoVideoServiceStatus(bool vimeoVideoServiceStatus)
        {
            VimeoVideoServiceStatus = vimeoVideoServiceStatus;
            return this;
        }

        public AppRoot SetAudioServiceStatus(bool audioServiceStatus)
        {
            AudioServiceStatus = audioServiceStatus;
            return this;
        }

        public AppRoot SetDocumentServiceStatus(bool documentServiceStatus)
        {
            DocumentServiceStatus = documentServiceStatus;
            return this;
        }

        #endregion
    }
}