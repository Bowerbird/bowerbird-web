using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.Commands;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserRequestPasswordResetCommand : CommandBase
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Email { get; set; }

        public string ResetPasswordKey { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion      
      
    }
}