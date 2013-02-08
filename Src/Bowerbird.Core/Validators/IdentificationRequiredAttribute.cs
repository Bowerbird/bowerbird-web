using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IdentificationRequiredAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = validationContext.ObjectInstance as IdentificationUpdateInput;

            if (input != null)
            {
                if (input.NewSighting || 
                    (!input.NewSighting && !string.IsNullOrWhiteSpace(input.SightingId)))
                {
                    if (input.IsCustomIdentification)
                    {
                        if (string.IsNullOrWhiteSpace(input.Kingdom) ||
                            string.IsNullOrWhiteSpace(input.Phylum) ||
                            string.IsNullOrWhiteSpace(input.Class) ||
                            string.IsNullOrWhiteSpace(input.Order) ||
                            string.IsNullOrWhiteSpace(input.Family) ||
                            string.IsNullOrWhiteSpace(input.Genus) ||
                            string.IsNullOrWhiteSpace(input.Species))
                        {
                            return new ValidationResult(ErrorMessageString);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(input.Taxonomy))
                        {
                            return new ValidationResult(ErrorMessageString);
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
