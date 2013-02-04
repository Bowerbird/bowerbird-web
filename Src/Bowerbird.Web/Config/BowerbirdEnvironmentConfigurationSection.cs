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

namespace Bowerbird.Web.Config
{
    public class BowerbirdEnvironmentConfigurationSection : ConfigurationSection
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [ConfigurationProperty("rootUri", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string RootUri
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

        #endregion

        #region Methods

        #endregion

    }
}
