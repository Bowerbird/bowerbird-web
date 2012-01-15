using System;
using System.Configuration;
using System.Collections;

namespace Bowerbird.Core.Config
{
    public class BowerbirdDocumentStoreConfigurationSection : ConfigurationSection
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [ConfigurationProperty("databaseName", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string DatabaseName
        {
            get
            {
                return (string)this["databaseName"];
            }
            set
            {
                this["databaseName"] = value;
            }
        }

        #endregion

        #region Methods

        #endregion

    }
}
