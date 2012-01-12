using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace Bowerbird.Web.ViewModels
{
    public class AccountRequestPasswordResetInput : IViewModel
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required(ErrorMessage = "Please enter you email address")]
        [Email(ErrorMessage = "The email entered is not valid")]
        public string Email { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
