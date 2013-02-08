using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DescriptionOrTagRequiredAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = validationContext.ObjectInstance as SightingNoteUpdateInput;

            if (input != null)
            {
                if (input.NewSighting ||
                    (!input.NewSighting && !string.IsNullOrWhiteSpace(input.SightingId)))
                {
                    if ((input.Descriptions == null ||
                        input.Descriptions.All(x => string.IsNullOrWhiteSpace(x.Value))) &&
                        string.IsNullOrWhiteSpace(input.Tags))
                    {
                        return new ValidationResult(ErrorMessageString);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
