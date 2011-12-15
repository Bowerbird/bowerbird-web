using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Bowerbird.Test.Utils
{
    public static class UtilityHelpers
    {
        public static T WaitForASecond<T>(this T t)
        {
            Thread.Sleep(1000);

            return t;
        }

        public static bool IsMoreRecentThan(this DateTime laterDate, DateTime earlierDate)
        {
            return laterDate > earlierDate;
        }

        /// <summary>
        /// Return the url path with querystring stripped
        /// </summary>
        public static string GetUrlFileName(this string url)
        {
            return url.Contains("?") ? url.Substring(0, url.IndexOf("?")) : url;
        }

        /// <summary>
        /// Return a NameValueCollection of name value pairs from the querystring
        /// </summary>
        public static NameValueCollection GetQueryStringParameters(this string url)
        {
            if (url.Contains("?"))
            {
                var parameters = new NameValueCollection();

                var parts = url.Split("?".ToCharArray());
                var keys = parts[1].Split("&".ToCharArray());

                foreach (var part in keys.Select(key => key.Split("=".ToCharArray())))
                    parameters.Add(part[0], part[1]);

                return parameters;
            }

            return null;
        }

        public static T MakeNull<T>(this object obj)
        {
            obj = null;

            return (T)obj;
        }

    }
}