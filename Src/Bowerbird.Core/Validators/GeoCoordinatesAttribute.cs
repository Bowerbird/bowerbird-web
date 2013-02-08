using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GeoCoordinatesAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = validationContext.ObjectInstance as ObservationUpdateInput;

            if (input != null)
            {
                if (string.IsNullOrWhiteSpace(input.Latitude) &&
                    string.IsNullOrWhiteSpace(input.Longitude))
                {
                    return ValidationResult.Success;
                }

                float latitude;
                float longitude;

                if (float.TryParse(input.Latitude, out latitude) &&
                    float.TryParse(input.Longitude, out longitude) &&
                    latitude >= -90 && 
                    latitude <= 90 &&
                    longitude >= -180 && 
                    longitude <= 180)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessageString);
        }
    }
}
