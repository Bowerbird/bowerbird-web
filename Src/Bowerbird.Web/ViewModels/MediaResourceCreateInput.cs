/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web;

namespace Bowerbird.Web.ViewModels
{
    public class MediaResourceCreateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// The key which is referenced client side upon creation
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Image, Video, etc
        /// </summary>
        public string MediaType { get; set; }

        public string Usage { get; set; }

        #region Images

        public string OriginalFileName { get; set; }

        public HttpPostedFileBase File { get; set; }

        #endregion

        #region Videos

        public string VideoUri { get; set; }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}