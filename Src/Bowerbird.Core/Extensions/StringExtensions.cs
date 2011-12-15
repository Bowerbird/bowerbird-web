using System.Text.RegularExpressions;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Extensions
{
    public static class StringExtensions
    {
        
        public static bool IsValidEmailAddress(this string emailAddress)
        {
            return new Regex(
                    @"^(([^<>()[\]\\.,;:\s@\""]+"
                  + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                  + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                  + @"[a-zA-Z]{2,}))$"
                ).IsMatch(emailAddress);
        }

        //public static string FormatWith(this string text, string param1)
        public static string FormatWith(this string text, string param1)
        {
            Check.Ensure(text.Contains("{0}"), "string was not in propper format");

            return string.Format(text, param1);
        }

        public static string AppendWith(this string text, string param1)
        {
            return string.Format("{0}{1}", text, param1);
        }

        public static string PrependWith(this string text, string param1)
        {
            return string.Format("{0}{1}", param1, text);
        }

    }
}