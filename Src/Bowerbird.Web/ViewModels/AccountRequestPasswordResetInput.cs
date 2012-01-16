using System.ComponentModel.DataAnnotations;
using Bowerbird.Web.Validators;
using DataAnnotationsExtensions;

namespace Bowerbird.Web.ViewModels
{
    public class AccountRequestPasswordResetInput
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "Please enter your email address")]
        [Email(ErrorMessage = "Please enter a valid email address")]
        [ValidEmail(ErrorMessage = "The email address does not exist, please enter another email address")]
        public string Email { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
