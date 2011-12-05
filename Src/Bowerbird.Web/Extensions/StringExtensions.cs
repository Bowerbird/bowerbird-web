using System.Collections.Generic;

namespace Bowerbird.Web.Extensions
{
    public static class StringExtensions
    {

        public static string FormatWith(this string text, string param1)
        {
            return string.Format(text, param1);
        }

        public static string FormatWith(this string text, string param1, string param2)
        {
            return string.Format(text, param1, param2);
        }

        public static string FormatWith(this string text, string param1, string param2, string param3)
        {
            return string.Format(text, param1, param2, param3);
        }

        public static string[] ToTokenizedCollection(this string text)
        {
            return text.Split(',');
        }

    }
}