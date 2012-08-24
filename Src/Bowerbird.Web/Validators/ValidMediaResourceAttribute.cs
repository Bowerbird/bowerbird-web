using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Validators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidMediaResourceAttribute : ValidationAttribute
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
            return usage == "contribution" || usage == "avatar";
        }

    }
}
