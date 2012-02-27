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

namespace Bowerbird.Core.Config
{
    public class BowerbirdDocumentStoreConfigurationSection : ConfigurationSection
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [ConfigurationProperty("url", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string DatabaseUrl
        {
            get
            {
                return (string)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }

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
