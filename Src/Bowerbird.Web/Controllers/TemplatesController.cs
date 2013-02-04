/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Caching;
using System.Collections.Generic;

namespace Bowerbird.Web.Controllers
{
    /// <summary>
    /// http://stackoverflow.com/questions/9702130/sharing-mustache-nustache-templates-between-server-and-client-asp-net-mvc
    /// </summary>
    public class TemplatesController : ControllerBase
    {
        #region Fields

        private List<string> _excludeTemplates = new List<string>() { "_Layout", "Error" };

        #endregion

        #region Constructors

        public TemplatesController()
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            var templates = LoadSharedMustacheTemplates();

            // Load all templates from Nustache
            foreach (var templateName in templates.Keys.ToList())
            {
                templates[templateName] = LoadTemplateSource(templateName);
            }

            // Concatenate all templates into a JSON array ie. "[{name: 'abc', source: '<html>'}]
            string templatesJson = string.Join(",",
                templates.Select(
                    x => string.Format("{{name: '{0}', source: '{1}'}}", x.Key, x.Value.Replace("\r\n", " ").Replace("'", "&#39;"))));

            // Return a JSONP result that contains the templates to be loaded by the client side ICanHaz.js template processor
            return Content(string.Format(@"
                define(['underscore', 'ich'], function (_, ich) {{
                    _.each([{0}], function(template) {{
                        ich.addTemplate(template.name, template.source);
                    }});
                }});
                ", templatesJson), "text/javascript; charset=UTF-8");
        }

        /// <summary>
        /// Iterate through the Shared Views template folder and load all mustache template names excluding _excludedTemplate names
        /// </summary>
        private Dictionary<string, string> LoadSharedMustacheTemplates()
        {
            var templates = new Dictionary<string, string>();

            var sharedTemplates = Directory
                .GetFiles(Server.MapPath("~/Views/Shared"))
                .Where(x => x.EndsWith(".mustache"))
                .Select(Path.GetFileNameWithoutExtension)
                .Where(x => !_excludeTemplates.Contains(x))
                .ToList();

            foreach (var templateName in sharedTemplates.Where(t => !templates.ContainsKey(t)))
            {
                templates.Add(templateName, null);
            }

            return templates;
        }

        /// <summary>
        /// This method loads a Nustache template from cache or file system. Directly ripped from https://github.com/jdiamond/Nustache/blob/master/Nustache.Mvc3/NustacheView.cs
        /// Note that this is a hack to allow us to load UN-RENDERED Nustache templates. If Nustache changes in the future, this method may stop working. Ideally,
        /// Nustache woudl have a way for us to call its internal version of this method (ie. NustacheView.LoadTemplate), or expose a "TemplateProvider" or some such thing.
        /// </summary>
        private string LoadTemplateSource(string path)
        {
            var key = "NustacheSourceTemplate:" + path;

            if (HttpContext.Cache[key] != null)
            {
                return (string)HttpContext.Cache[key];
            }

            var templatePath = HttpContext.Server.MapPath(string.Format("/Views/Shared/{0}.mustache", path));
            var templateSource = System.IO.File.ReadAllText(templatePath);

            HttpContext.Cache.Insert(key, templateSource, new CacheDependency(templatePath));

            return (string)HttpContext.Cache[key];
        }

        #endregion
    }
}