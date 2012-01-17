using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Ninject;
using Raven.Client;

namespace Bowerbird.Web.Validators
{
    /// <summary>
    /// Checks whether specified email exists
    /// </summary>
    public class ValidEmailAttribute : ValidationAttribute
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        #endregion

        #region Methods

        #endregion      
      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = string.IsNullOrEmpty(value as string) ? string.Empty : value as string;

            if (!DocumentSession
                    .Query<User>()
                    .Any(x => x.Email == email))
            {
                return new ValidationResult(ErrorMessageString);
            }

            return ValidationResult.Success;
        }

    }
}
