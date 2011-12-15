using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Extensions
{
    public static class StringExtensions
    {

        public static string FormatWith(this string text, string param1)
        {
            Check.Require(text.Contains("{0}"), text + " does not contain the placeholder '{0}'");

            return string.Format(text, param1);
        }

        public static string FormatWith(this string text, string param1, string param2)
        {
            Check.Require(text.Contains("{0}"), text + " does not contain the placeholder '{0}'");

            return string.Format(text, param1, param2);
        }

        public static string FormatWith(this string text, string param1, string param2, string param3)
        {
            Check.Require(text.Contains("{0}"), text + " does not contain the placeholder '{0}'");

            return string.Format(text, param1, param2, param3);
        }

        public static string[] ToTokenizedCollection(this string text)
        {
            Check.Require(text != null, "text may not be null");

            return text.Split(',');
        }

    }
}