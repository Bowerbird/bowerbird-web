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

        // Common properties for all MediaResource types
        public string UserId { get; set; }

        public DateTime UploadedOn { get; set; }

        public string Key { get; set; } // temporary value but persisted for asynch round-trip

        public string MediaType { get; set; } // video, image, document...

        public string ReccordType { get; set; } // observation, avatar, project...

        // Image MediaResource properties
        public string OriginalFileName { get; set; }

        public Stream Stream { get; set; }

        public string Usage { get; set; }

        // Video MediaResource properties
        public string LinkUri { get; set; }

        public string Description { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}
