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
    public class ValidMediaAttribute : ValidationAttribute
    {

        public override Boolean IsValid(Object value)
        {
            if(value != null && value is MediaResourceCreateInput)
            {
                var input = (MediaResourceCreateInput) value;
                return IsValidType(input.Type.ToLower()) && IsValidUsage(input.Usage.ToLower()) && IsValidMedia(input);
            }

            return false;
        }

        private bool IsValidType(string type)
        {
            return type == "file" || type == "externalvideo";
        }

        private bool IsValidUsage(string usage)
        {
            return usage == "contribution" || usage == "avatar";
        }

        private bool IsValidMedia(MediaResourceCreateInput input)
        {
            // If the media is an uploaded file
            if(input.Type.ToLower() == "file")
            {
                return input.File != null && MediaTypeUtility.IsSupportedFile(input.File.InputStream);
            }

            // If the media is an imported video
            if(input.Type.ToLower() == "externalvideo")
            {
                return
                    !string.IsNullOrWhiteSpace(input.VideoProviderName) &&
                    !string.IsNullOrWhiteSpace(input.VideoId) &&
                    (input.VideoProviderName.ToLower() == "youtube" || input.VideoProviderName.ToLower() == "vimeo");
            }

            return false;
        }
    }
}
