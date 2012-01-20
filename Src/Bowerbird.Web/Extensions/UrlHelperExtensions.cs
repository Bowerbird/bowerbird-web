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

using System.Configuration;
using System.Web.Mvc;
using Bowerbird.Core.Extensions;
				
namespace Bowerbird.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Load an image file from the default img folder
        /// </summary>
        public static string Image(this UrlHelper helper, string fileName)
        {
            return helper.Content("~/img/{0}".FormatWith(fileName));
        }

        /// <summary>
        /// Load an image file from a custom folder within the img folder
        /// </summary>
        public static string Image(this UrlHelper helper, string path, string fileName)
        {
            return helper.Content("~/img/{0}/{1}".FormatWith(path, fileName));
        }

        /// <summary>
        /// Specify a stylesheet within the default css folder
        /// </summary>
        public static string Stylesheet(this UrlHelper helper, string fileName)
        {
            return helper.Content("~/css/{0}?v={1}".FormatWith(fileName, ConfigurationManager.AppSettings["StaticContentIncrement"]));
        }

        /// <summary>
        /// Load a javascript file from the js folder
        /// Used for sitewide scripts
        /// </summary>
        public static string JavaScript(this UrlHelper helper, string fileName)
        {
            return helper.Content("~/js/{0}?v={1}".FormatWith(fileName, ConfigurationManager.AppSettings["StaticContentIncrement"]));
        }

        /// <summary>
        /// Load a javascript file from the js/libs folder
        /// Used for third party library scripts
        /// </summary>
        public static string JavaScriptLib(this UrlHelper helper, string fileName)
        {
            return helper.Content("~/js/libs/{0}?v={1}".FormatWith(fileName, ConfigurationManager.AppSettings["StaticContentIncrement"]));
        }

        /// <summary>
        /// Load a javascript file from the js/bowerbird folder.
        /// Used for custom page scripts
        /// </summary>
        public static string JavaScriptFile(this UrlHelper helper, string folder, string fileName)
        {
            return helper.Content("~/js/bowerbird/{0}/{1}?v={2}".FormatWith(folder, fileName, ConfigurationManager.AppSettings["StaticContentIncrement"]));
        }

        /// <summary>
        /// Load a javascript file from the js/bowerbird folder.
        /// Used for custom page scripts
        /// </summary>
        public static string JavaScriptFile(this UrlHelper helper, string fileName)
        {
            return helper.Content("~/js/bowerbird/{0}?v={1}".FormatWith(fileName, ConfigurationManager.AppSettings["StaticContentIncrement"]));
        }

    }
}