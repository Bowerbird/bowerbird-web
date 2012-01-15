using System;
using System.Configuration;
using System.Collections;

namespace Bowerbird.Core.Config
{
    public class BowerbirdMediaConfigurationSection : ConfigurationSection
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [ConfigurationProperty("rootUri", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string MediaRootUri
        {
            get
            {
                return (string)this["rootUri"];
            }
            set
            {
                this["rootUri"] = value;
            }
        }

        [ConfigurationProperty("rootPath", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string MediaRootPath
        {
            get
            {
                return (string)this["rootPath"];
            }
            set
            {
                this["rootPath"] = value;
            }
        }

        #endregion

        #region Methods

        #endregion

    }
}
