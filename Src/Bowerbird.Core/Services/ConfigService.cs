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

namespace Bowerbird.Core.Services
{
    public class ConfigService : IConfigService
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public string GetEmailServerName()
        {
            return ((BowerbirdEmailConfigurationSection)ConfigurationManager.GetSection("bowerbird/email")).ServerName;
        }

        public string GetMediaRootUri()
        {
            return ((BowerbirdMediaConfigurationSection)ConfigurationManager.GetSection("bowerbird/media")).MediaRootUri;
        }

        public string GetMediaRootPath()
        {
            return ((BowerbirdMediaConfigurationSection)ConfigurationManager.GetSection("bowerbird/media")).MediaRootPath;
        }

        public string GetDatabaseName()
        {
            return ((BowerbirdDocumentStoreConfigurationSection)ConfigurationManager.GetSection("bowerbird/documentStore")).DatabaseName;
        }

        public string GetEmailAdminAccount()
        {
            return ((BowerbirdEmailConfigurationSection)ConfigurationManager.GetSection("bowerbird/email")).AdminAccount;
        }

        public string GetEmailResetPasswordUri()
        {
            return ((BowerbirdEmailConfigurationSection)ConfigurationManager.GetSection("bowerbird/email")).ResetPasswordUri;
        }

        #endregion
     
    }
}
