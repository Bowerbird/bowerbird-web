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
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Bowerbird.Web.Chat.Infrastructure;

namespace Bowerbird.Web.Chat.ContentProviders
{
    public class ResourceProcessor : IResourceProcessor
    {
        private readonly Lazy<IList<IContentProvider>> _contentProviders = new Lazy<IList<IContentProvider>>(GetContentProviders);

        public Task<ContentProviderResult> ExtractResource(string url)
        {
            Uri resultUrl;
            if (Uri.TryCreate(url, UriKind.Absolute, out resultUrl))
            {
                var request = new ContentProviderHttpRequest(resultUrl);
                return ExtractContent(request);
            }

            return TaskAsyncHelper.FromResult<ContentProviderResult>(null);
        }

        private Task<ContentProviderResult> ExtractContent(ContentProviderHttpRequest request)
        {
            var contentProviders = _contentProviders.Value;

            var validProviders = contentProviders.Where(c => c.IsValidContent(request.RequestUri))
                                                 .ToList();

            if (validProviders.Count == 0)
            {
                return TaskAsyncHelper.FromResult<ContentProviderResult>(null);
            }

            var tasks = validProviders.Select(c => c.GetContent(request)).ToArray();

            var tcs = new TaskCompletionSource<ContentProviderResult>();

            Task.Factory.ContinueWhenAll(tasks, completedTasks =>
            {
                var faulted = completedTasks.FirstOrDefault(t => t.IsFaulted);
                if (faulted != null)
                {
                    tcs.SetException(faulted.Exception);
                }
                else if (completedTasks.Any(t => t.IsCanceled))
                {
                    tcs.SetCanceled();
                }
                else
                {
                    ContentProviderResult result = completedTasks.Select(t => t.Result)
                                                                 .FirstOrDefault(content => content != null);
                    tcs.SetResult(result);
                }
            });

            return tcs.Task;
        }


        private static IList<IContentProvider> GetContentProviders()
        {
            // Use MEF to locate the content providers in this assembly
            var compositionContainer = new CompositionContainer(new AssemblyCatalog(typeof(ResourceProcessor).Assembly));
            return compositionContainer.GetExportedValues<IContentProvider>().ToList();
        }
    }
}