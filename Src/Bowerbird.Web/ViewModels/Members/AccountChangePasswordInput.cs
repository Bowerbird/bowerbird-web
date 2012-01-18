using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bowerbird.Web.ViewModels.Members
{
    public class AccountChangePasswordInput
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "Please enter a password")]
        [StringLength(1000, MinimumLength = 6, ErrorMessage = "Passwords must be at least 6 characters in length")]
        public string Password { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
