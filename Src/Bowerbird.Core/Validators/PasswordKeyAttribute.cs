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
using NLog;
using Ninject;
using Raven.Client;

namespace Bowerbird.Core.Validators
{
    /// <summary>
    /// Checks whether specified change password key exists
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordKeyAttribute : ValidationAttribute
    {
            
        #region Members

        private Logger _logger = LogManager.GetLogger("PasswordKeyAttribute");

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
            _logger.Debug("Validating password key. Key: {0}.", value);

            string resetPasswordKey = string.IsNullOrEmpty(value as string) ? string.Empty : value as string;

            if (string.IsNullOrWhiteSpace(resetPasswordKey))
            {
                _logger.Debug("Validating password key. Key is empty.");

                return new ValidationResult(ErrorMessageString);
            }

            if (!DocumentSession
                    .Query<User>()
                    .Any(x => x.ResetPasswordKey == resetPasswordKey))
            {
                _logger.Debug("Validating password key. Key: {0}. Key does not exist in database.", value);

                return new ValidationResult(ErrorMessageString);
            }

            _logger.Debug("Validating password key. Key: {0}. Key exists.", value);

            return ValidationResult.Success;
        }

        #endregion      

    }
}
