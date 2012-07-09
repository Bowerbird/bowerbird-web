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
using Bowerbird.Core.Config;
using Bowerbird.Core.Services;
using System.Web;
using Bowerbird.Core.DesignByContract;
using System;

namespace Bowerbird.Web.Services
{
    public class ConfigSettings : IConfigSettings
    {
            
        #region Members

        //private readonly HttpContextBase _httpContext;

        #endregion

        #region Constructors

        //public ConfigService(HttpContextBase httpContext)
        //{
        //    Check.RequireNotNull(httpContext, "httpContext");

        //    _httpContext = httpContext;
        //}

        public ConfigSettings()
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public string GetEnvironmentRootPath()
        {
            string path = string.Empty;

            if (HttpContext.Current != null)
            {
                path = HttpContext.Current.Server.MapPath("/");
            }
            else
            {
                throw new Exception("We don't have access to SignalR host context! How?????");
                // Get it from SignalR
                //hostContext.Items["System.Web.HttpContext"]
            }

            if (path.EndsWith("\\bin\\Debug"))
            {
                path = path.Replace("\\bin\\Debug", string.Empty);
            }

            return path;

            //string path = _httpContext.Server.MapPath("/");

            //if (path.EndsWith("\\bin\\Debug"))
            //{
            //    path = path.Replace("\\bin\\Debug", string.Empty);
            //}

            //return path;
        }

        public string GetEnvironmentRootUri()
        {
            return ((BowerbirdEnvironmentConfigurationSection)ConfigurationManager.GetSection("bowerbird/environment")).RootUri;
        }

        public string GetEmailServerName()
        {
            return ((BowerbirdEmailConfigurationSection)ConfigurationManager.GetSection("bowerbird/email")).ServerName;
        }

        public string GetMediaRootUri()
        {
            return ((BowerbirdMediaConfigurationSection)ConfigurationManager.GetSection("bowerbird/media")).MediaRootUri;
        }

        public string GetMediaRelativePath()
        {
            return ((BowerbirdMediaConfigurationSection)ConfigurationManager.GetSection("bowerbird/media")).MediaRelativePath;
        }

        public string GetDatabaseUrl()
        {
            return ((BowerbirdDocumentStoreConfigurationSection)ConfigurationManager.GetSection("bowerbird/documentStore")).DatabaseUrl;
        }

        public string GetDatabaseName()
        {
            return ((BowerbirdDocumentStoreConfigurationSection)ConfigurationManager.GetSection("bowerbird/documentStore")).DatabaseName;
        }

        public string GetEmailAdminAccount()
        {
            return ((BowerbirdEmailConfigurationSection)ConfigurationManager.GetSection("bowerbird/email")).AdminAccount;
        }

        public string GetEmailResetPasswordRelativeUri()
        {
            return ((BowerbirdEmailConfigurationSection)ConfigurationManager.GetSection("bowerbird/email")).ResetPasswordRelativeUri;
        }

        public string GetSpeciesRelativePath()
        {
            return ((BowerbirdSpeciesConfigurationSection)ConfigurationManager.GetSection("bowerbird/species")).RelativePath;
        }

        #endregion
        
    }
}
