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

namespace Bowerbird.Core.Commands
{
    public class ImageMediaResourceCreateCommand : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public DateTime UploadedOn { get; set; }

        public string OriginalFileName { get; set; }

        public string FileFormat { get; set; }

        public string Description { get; set; }

        public int OriginalHeight { get; set; }

        public int OriginalWidth { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}
