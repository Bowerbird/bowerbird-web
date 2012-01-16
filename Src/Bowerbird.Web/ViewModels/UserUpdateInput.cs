using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Bowerbird.Web.Validators;
using DataAnnotationsExtensions;

namespace Bowerbird.Web.ViewModels
{
    public class UserUpdateInput
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [Email(ErrorMessage = "Please enter a valid email address")]
        [UniqueEmail(ErrorMessage = "The email address already exists, please enter another email address", IgnoreAuthenticatedUserEmail = true)]
        public string Email { get; set; }

        public string Description { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
