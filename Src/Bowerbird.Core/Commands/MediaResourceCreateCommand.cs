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

using System;
using System.IO;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.Commands
{
    public class MediaResourceCreateCommand : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Client side idenitifier
        /// </summary>
        public string Key { get; set; }

        public string UserId { get; set; }

        public DateTime UploadedOn { get; set; }

        /// <summary>
        /// Image, Video, Document, etc
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// How the media resource will be used
        /// </summary>
        public string Usage { get; set; }

        #region Images

        public string OriginalFileName { get; set; }

        public Stream Stream { get; set; }

        #endregion

        #region Videos

        public string VideoUri { get; set; }

        #endregion

        #endregion

        #region Methods

        #endregion

    }
}
