/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia

Bowerbird.Web.Chat and sub namespaces have the following attribution as sourced from https://github.com/davidfowl/JabbR/

- Copyright (c) 2011 David Fowler
- Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
- The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Bowerbird.Web.Chat.Models;

namespace Bowerbird.Web.Chat.Infrastructure
{

    public class TextTransform
    {
        private readonly IChatRepository _repository;
        public const string HashTagPattern = @"(?:(?<=\s)|^)#([A-Za-z0-9-_.]{1,30}\w*)";

        public TextTransform(IChatRepository repository)
        {
            _repository = repository;
        }

        public string Parse(string message)
        {
            return ConvertTextWithNewLines(ConvertHashtagsToRoomLinks(message));
        }

        private string ConvertTextWithNewLines(string message)
        {
            // If the message contains new lines wrap all of it in a pre tag
            if (message.Contains('\n'))
            {
                return String.Format(@"
<div class=""collapsible_content"">
    <h3 class=""collapsible_title"">Paste (click to show/hide)</h3>
    <div class=""collapsible_box"">
        <pre class=""multiline"">{0}</pre>
    </div>
</div>
", message);
            }

            return message;
        }

        public static string TransformAndExtractUrls(string message, out HashSet<string> extractedUrls)
        {
            const string urlPattern = @"(?i)(?<s>(?:https?|ftp)://|www\.)?(?:\S+(?::\S*)?@)?(?:(?:[\w\p{S}][\w\p{S}@-]*[.:])+\w+)(?(s)/?|/)(?:(?:[^\s()<>.,\u0022'”]+|[.,\u0022'”][^\s()<>.,\u0022]|\((?:[^\s()<>]+|(?:\([^\s()<>]+\)))*\))*)";

            var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            message = Regex.Replace(message, urlPattern, m =>
            {

                string httpPortion = String.Empty;
                if (!m.Value.Contains("://"))
                {
                    httpPortion = "http://";
                }

                string url = httpPortion + m.Value;

                urls.Add(HttpUtility.HtmlDecode(url));

                return String.Format(CultureInfo.InvariantCulture,
                                     "<a rel=\"nofollow external\" target=\"_blank\" href=\"{0}\" title=\"{1}\">{1}</a>",
                                     url, m.Value);
            });

            extractedUrls = urls;
            return message;
        }

        public string ConvertHashtagsToRoomLinks(string message)
        {
            message = Regex.Replace(message, HashTagPattern, m =>
            {
                //hashtag without #
                string groupId = m.Groups[1].Value;

                var room = _repository.GetGroupById(groupId);

                if (room != null)
                {
                    return String.Format(CultureInfo.InvariantCulture,
                                         "<a href=\"#/rooms/{0}\" title=\"{1}\">{1}</a>",
                                         groupId,
                                         m.Value);
                }

                return m.Value;
            });

            return message;
        }

    }
}