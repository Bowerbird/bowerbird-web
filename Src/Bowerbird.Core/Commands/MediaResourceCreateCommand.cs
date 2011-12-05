using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class MediaResourceCreateCommand : CommandBase
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Type { get; set; }

        public string OriginalFilename { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion      
      
    }
}
