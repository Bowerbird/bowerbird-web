using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;
using Ninject;
using Raven.Client;

namespace Bowerbird.Web.Validators
{
    /// <summary>
    /// Checks whether email address is available (unique)
    /// </summary>
    public class UniqueEmailAttribute : ValidationAttribute
    {
            
        #region Members

        #endregion

        #region Constructors

        public UniqueEmailAttribute()
            : base()
        {
            InitMembers();
        }

        public UniqueEmailAttribute(Func<string> errorMessageAccessor)
            : base(errorMessageAccessor)
        {
            InitMembers();
        }

        #endregion

        #region Properties

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        [Inject]
        public IUserContext UserContext { get; set; }

        /// <summary>
        /// Setting IgnoreAuthenticatedUserEmail will ignore the user's current email address while searching for unique email addresses. Default is false.
        /// </summary>
        public bool IgnoreAuthenticatedUserEmail { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            IgnoreAuthenticatedUserEmail = false;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = string.IsNullOrEmpty(value as string) ? string.Empty : value as string;

            if (IgnoreAuthenticatedUserEmail)
            {
                string authenticatedUserId = UserContext.GetAuthenticatedUserId();

                if (DocumentSession.Query<User>()
                    .Any(x => x.Id != authenticatedUserId && x.Email == email))
                {
                    return new ValidationResult(ErrorMessageString);
                } 
            }
            else
            {
                if (DocumentSession.Query<User>()
                        .Any(x => x.Email == email))
                {
                    return new ValidationResult(ErrorMessageString);
                }                
            }

            return ValidationResult.Success;
        }

        #endregion

    }
}
