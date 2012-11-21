/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
                
namespace Bowerbird.Web.ViewModels
{
    /// <summary>
    /// Create/update an observation media item
    /// </summary>
    public class ObservationMediaItem
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Specify MediaResourceId if it has been created prior
        /// </summary>
        public string MediaResourceId { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public string Licence { get; set; }

        public bool IsPrimaryMedia { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}