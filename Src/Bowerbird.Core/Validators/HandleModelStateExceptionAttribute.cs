/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Text;
using System.Web.Mvc;

namespace Bowerbird.Core.Validators
{
    /// <summary>
    /// Represents errors that occur due to invalid application model state.
    /// 
    /// This Code is sourced from Robert Koritnik:
    /// http://erraticdev.blogspot.com/2010/11/handling-validation-errors-on-ajax.html
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HandleModelStateExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// Called when an exception occurs and processes <see cref="ModelStateException"/> object.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            // handle modelStateException
            if (filterContext.Exception != null && typeof(ModelStateException).IsInstanceOfType(filterContext.Exception) && !filterContext.ExceptionHandled)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.ContentEncoding = Encoding.UTF8;
                filterContext.HttpContext.Response.HeaderEncoding = Encoding.UTF8;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.HttpContext.Response.StatusCode = 400;
                filterContext.Result = new ContentResult
                                           {
                                               Content = (filterContext.Exception as ModelStateException).Message,
                                               ContentEncoding = Encoding.UTF8,
                                           };
            }
        }
    }
}