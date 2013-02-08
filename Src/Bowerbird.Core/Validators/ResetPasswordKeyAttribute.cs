/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Ninject;
using Raven.Client;

namespace Bowerbird.Core.Validators
{
    /// <summary>
    /// Checks whether specified reset password key exists
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResetPasswordKeyAttribute : ValidationAttribute
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

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string resetPasswordKey = string.IsNullOrEmpty(value as string) ? string.Empty : value as string;

            if (DocumentSession
                    .Query<User>()
                    .All(x => x.ResetPasswordKey != resetPasswordKey))
            {
                return new ValidationResult(ErrorMessageString);
            }

            return ValidationResult.Success;
        }

        #endregion      

    }
}
