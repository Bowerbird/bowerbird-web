using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Commands
{
    public class ImageMediaResourceCreateCommand : CommandBase
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public DateTime UploadedOn { get; set; }

        public string OriginalFileName { get; set; }

        public string FileFormat { get; set; }

        public string Description { get; set; }

        public int OriginalHeight { get; set; }

        public int OriginalWidth { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
