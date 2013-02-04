using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Validators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidExternalVideoAttribute : ValidationAttribute
    {

        /// <summary>
        /// A valid video is one which has a video provider name and videoid
        /// </summary>
        public override bool IsValid(Object value)
        {
            var input = value as MediaResourceCreateInput;

            if (input == null || input.Type.ToLower() != "externalvideo") return true;

            return
                !string.IsNullOrWhiteSpace(input.VideoProviderName) &&
                !string.IsNullOrWhiteSpace(input.VideoId) &&
                (input.VideoProviderName.ToLower() == "youtube" || input.VideoProviderName.ToLower() == "vimeo");
        }

        //var uriRegExp = /^(?:https?:\/\/)?(?:www\.)?youtube\.com\/watch\?(?=.*v=((\w|-){11}))(?:\S+)?$/
        //private bool IsValidUrl(string url)
        //{
        //    var youtubeUrlRegExp = new System.Text.RegularExpressions.Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
        //    var vimeoUrlRegExp = new System.Text.RegularExpressions.Regex(@"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)");

        //    return youtubeUrlRegExp.Match(url).Success || vimeoUrlRegExp.Match(url).Success;
        //}

    }
}
