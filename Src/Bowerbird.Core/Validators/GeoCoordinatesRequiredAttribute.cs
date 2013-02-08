using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GeoCoordinatesRequiredAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = validationContext.ObjectInstance as ObservationUpdateInput;

            if (input != null)
            {
                if (string.IsNullOrWhiteSpace(input.Latitude) &&
                    string.IsNullOrWhiteSpace(input.Longitude))
                {
                    return new ValidationResult(ErrorMessageString);
                }
            }

            return ValidationResult.Success;
        }
    }
}
