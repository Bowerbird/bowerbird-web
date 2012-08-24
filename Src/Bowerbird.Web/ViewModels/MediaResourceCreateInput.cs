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
using System.ComponentModel.DataAnnotations;
using Bowerbird.Web.Validators;

namespace Bowerbird.Web.ViewModels
{
    [ValidMediaResource]
    [ValidFile(ErrorMessage = "The uploaded file is not a supported type. The supported image types are JPEG, PNG, TIFF, BMP and GIF. The supported audio types are M4A and MP3. The supported document types are DOC, TXT and PDF. Please check the file and try again.")]
    [ValidExternalVideo(ErrorMessage = "The imported video is not valid. Videos can only be imported from Youtube and Vimeo. Please check the URL and try again.")]
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
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Type of media (file, externalvideo)
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// How the media will be used (contribution, avatar)
        /// </summary>
        [Required]
        public string Usage { get; set; }

        #region File

        public string FileName { get; set; }

        public HttpPostedFileBase File { get; set; }

        #endregion

        #region External Video

        public string VideoProviderName { get; set; }

        public string VideoId { get; set; }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}