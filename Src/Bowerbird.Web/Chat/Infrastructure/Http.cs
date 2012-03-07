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
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bowerbird.Web.Chat.Infrastructure
{
    public static class Http
    {
        private const string _userAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; MAAU)";

        public static Task<object> GetJsonAsync(string url)
        {
            var task = GetAsync(url, webRequest =>
            {
                webRequest.Accept = "application/json";
            });

            return task.Then(response =>
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return JsonConvert.DeserializeObject(reader.ReadToEnd());
                }
            });
        }

        public static Task<HttpWebResponse> GetAsync(Uri uri, Action<HttpWebRequest> init = null)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.UserAgent = _userAgent;
            if (init != null)
            {
                init(request);
            }

            return Task.Factory.FromAsync((cb, state) => request.BeginGetResponse(cb, state), ar => (HttpWebResponse)request.EndGetResponse(ar), null);
        }

        public static Task<HttpWebResponse> GetAsync(string url, Action<HttpWebRequest> init = null)
        {
            return GetAsync(new Uri(url), init);
        }

        public static Task<TResult> GetJsonAsync<TResult>(string url)
        {
            var task = GetAsync(url, webRequest =>
            {
                webRequest.Accept = "application/json";
            });

            return task.Then(response =>
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return JsonConvert.DeserializeObject<TResult>(reader.ReadToEnd());
                }
            });
        }
    }
}