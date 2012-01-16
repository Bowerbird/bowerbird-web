using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using FluentEmail;

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
