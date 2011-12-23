using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override ICollection<System.ComponentModel.DataAnnotations.ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion      
      
    }
}
