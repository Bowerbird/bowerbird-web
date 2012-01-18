using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Bowerbird.Web.Validators;

namespace Bowerbird.Web.ViewModels.Public
{
    
    public class AccountResetPasswordInput
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [ValidResetPasswordKeyRequest(ErrorMessage = "The password reset request is not valid")]
        public string ResetPasswordKey { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
