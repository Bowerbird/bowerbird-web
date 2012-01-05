using Bowerbird.Core.DesignByContract;
using System;

namespace Bowerbird.Core.Entities
{
    public abstract class MediaResource : Entity
    {

        #region Members

        #endregion

        #region Constructors

        protected MediaResource() :base() { }

        protected MediaResource(
            string originalFileName,
            string fileFormat,
            string description)
            : this()
        {
            Check.RequireNotNullOrWhitespace(originalFileName, "originalFileName");
            Check.RequireNotNullOrWhitespace(fileFormat, "fileFormat");

            SetDetails(
                originalFileName,
                fileFormat,
                description);
        }

        #endregion

        #region Properties

        public string OriginalFileName { get; private set; }

        public string FileFormat { get; private set; }

        public string Description { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string originalFileName, string fileFormat, string description)
        {
            Id = Guid.NewGuid().ToString();
            OriginalFileName = originalFileName;
            FileFormat = fileFormat;
            Description = description;
        }

        #endregion      
      
    }
}