/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
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

        public static string AppendWith(this string text, string param1)
        {
            return string.Format("{0}{1}", text, param1);
        }

        public static string PrependWith(this string text, string param1)
        {
            return string.Format("{0}{1}", param1, text);
        }

        public static string[] ToTokenizedCollection(this string text)
        {
            Check.Require(text != null, "text may not be null");

            return text.Split(',');
        }

        public static string FileSizeDisplay(this long fileSizeInBytes)
        {
            const decimal megaByte = 1048576;
            const decimal kiloByte = 1024;

            decimal adjustedResult = fileSizeInBytes;
            var unit = "b";

            if(fileSizeInBytes/megaByte > 1)
            {
                adjustedResult = fileSizeInBytes/megaByte;
                unit = "Mb";
            }
            else if(fileSizeInBytes/kiloByte > 1)
            {
                adjustedResult = fileSizeInBytes/kiloByte;
                unit = "Kb";
            }

            return string.Format("{0}{1}", Math.Round(adjustedResult, 1), unit);
        }

        public static bool EqualsIgnoreCase(this string text, string compare)
        {
            return text.ToLower().Equals(compare.ToLower());
        }

        public static string ShortId(this string longId)
        {
            if(longId.Contains("/")) 
                
                return longId.Split('/')[1];

            return longId;
        }

        public static string FormatWith(this string text, string param1, string param2)
        {
            return string.Format("{0}{1}{2}", text, param1, param2);
        }
    }
}