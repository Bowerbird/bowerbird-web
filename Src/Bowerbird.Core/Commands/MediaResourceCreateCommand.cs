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
        /// Type of media (File, ExternalVideo)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// How the media will be used (Contribution, Avatar)
        /// </summary>
        public string Usage { get; set; }

        #region File

        public string FileName { get; set; }

        public Stream FileStream { get; set; }

        #endregion

        #region ExternalVideo

        public string VideoProviderName { get; set; }

        public string VideoId { get; set; }

        #endregion

        #endregion

        #region Methods

        #endregion

    }
}
