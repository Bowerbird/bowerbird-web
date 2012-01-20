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
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public abstract class MediaResource : DomainModel
    {

        #region Members

        #endregion

        #region Constructors

        protected MediaResource() :base() { }

        protected MediaResource(
            User createdByUser,
            DateTime uploadedOn,
            string originalFileName,
            string fileFormat,
            string description)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(originalFileName, "originalFileName");
            Check.RequireNotNullOrWhitespace(fileFormat, "fileFormat");

            SetDetails(
                createdByUser,
                uploadedOn,
                originalFileName,
                fileFormat,
                description);
        }

        #endregion

        #region Properties

        public DenormalisedUserReference CreatedByUser { get; set; }

        public string OriginalFileName { get; private set; }

        public string FileFormat { get; private set; }

        public string Description { get; private set; }

        public DateTime UploadedOn { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(User createdByUser, DateTime uploadedOn, string originalFileName, string fileFormat, string description)
        {
            Id = Guid.NewGuid().ToString();
            UploadedOn = uploadedOn;
            CreatedByUser = createdByUser;
            OriginalFileName = originalFileName;
            FileFormat = fileFormat;
            Description = description;
        }

        protected void UpdateDetails(string description)
        {
            Description = description;
        }

        #endregion      
      
    }
}