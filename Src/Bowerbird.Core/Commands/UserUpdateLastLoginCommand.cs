using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.Commands;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserUpdateLastLoginCommand : CommandBase
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion      
      
    }
}