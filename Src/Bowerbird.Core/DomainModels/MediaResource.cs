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
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class MediaResource : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected MediaResource() :base() { }

        public MediaResource(
            string type,
            User createdByUser,
            DateTime uploadedOn,
            IDictionary<string, string> metadata)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(metadata, "metadata");

            Type = type;
            UploadedOn = uploadedOn;
            CreatedByUser = createdByUser;
            Metadata = metadata;
        }

        #endregion

        #region Properties

        public string Type { get; private set; }

        public DenormalisedUserReference CreatedByUser { get; private set; }

        public DateTime UploadedOn { get; private set; }

        public IDictionary<string, string> Metadata { get; private set; }

        #endregion

        #region Methods

        #endregion      
    }
}