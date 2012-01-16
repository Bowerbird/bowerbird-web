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

namespace Bowerbird.Core.Extensions
{
    #region Namespaces

    using System.IO;
    using System.Reflection;
    using FluentEmail;

    #endregion

    public static class EmailExtensions
    {
        public static Email UsingTemplateFromResource<T>(this Email email, string templateResourceName, T model, bool isHtml = true)
        {
            string template = null;

            Assembly assembly = Assembly.GetAssembly(typeof (EmailExtensions));
            using (Stream stream = assembly.GetManifestResourceStream(string.Format("Bowerbird.Core.EmailTemplates.{0}.html", templateResourceName)))
            {
                using (var reader = new StreamReader(stream))
                {
                    template = reader.ReadToEnd();
                }
            }

            return email.UsingTemplate(template, model, isHtml);
        }
    }
}