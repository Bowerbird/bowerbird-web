using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Commands
{
    public class UserUpdatePasswordCommand : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string ResetPasswordKey { get; set; }

        public string UserId { get; set; }
    
        public string Password { get; set; }

        #endregion

        #region Methods

        #endregion      
      
        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public ICollection<System.ComponentModel.DataAnnotations.ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }
    }
}
