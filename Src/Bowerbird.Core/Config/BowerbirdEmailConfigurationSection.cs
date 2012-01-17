using System;
using System.Configuration;
using System.Collections;

namespace Bowerbird.Core.Config
{
    public class BowerbirdEmailConfigurationSection : ConfigurationSection
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [ConfigurationProperty("serverName", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string ServerName
        {
            get
            {
                return (string)this["serverName"];
            }
            set
            {
                this["serverName"] = value;
            }
        }

        [ConfigurationProperty("adminAccount", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string AdminAccount
        {
            get
            {
                return (string)this["adminAccount"];
            }
            set
            {
                this["adminAccount"] = value;
            }
        }

        [ConfigurationProperty("resetPasswordUri", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string ResetPasswordUri
        {
            get
            {
                return (string)this["resetPasswordUri"];
            }
            set
            {
                this["resetPasswordUri"] = value;
            }
        }

        #endregion

        #region Methods

        #endregion

    }
}
