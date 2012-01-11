using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class ProjectDeleteCommand : CommandBase
    {
        #region Fields

        #endregion

        #region Constructors

        public ProjectDeleteCommand()
        {
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}