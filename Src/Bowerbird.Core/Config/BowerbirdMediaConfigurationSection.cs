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
