using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Validators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MediaResourceUsageAttribute : ValidationAttribute
    {

        public override bool IsValid(Object value)
        {
            if (value == null) return true;

            var input = value as MediaResourceCreateInput;
            
            return IsValidType(input.Type.ToLower()) && IsValidUsage(input.Usage.ToLower());
        }

        private bool IsValidType(string type)
        {
            return type == "file" || type == "externalvideo";
        }

        private bool IsValidUsage(string usage)
        {
            return usage == "contribution" || usage == "avatar" || usage == "background";
        }

    }
}
