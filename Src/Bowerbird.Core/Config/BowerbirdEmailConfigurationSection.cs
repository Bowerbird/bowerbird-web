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

        #endregion

        #region Methods

        #endregion

    }
}
