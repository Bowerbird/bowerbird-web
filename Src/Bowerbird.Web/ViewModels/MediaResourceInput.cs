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
    public class MediaResourceInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        // Used for all Media Resources
        public string Key { get; set; }

        public string Description { get; set; }

        public string MediaType { get; set; }

        // Used for Images
        public string OriginalFileName { get; set; }

        public HttpPostedFileBase File { get; set; }

        public string Usage { get; set; }

        // Used for Videos 
        public string LinkUri { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}