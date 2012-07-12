/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.ImageUtilities
{
    internal class ImageCreationTask
    {
        public MediaResourceFile File { get; set; }
        public string StoredRepresentation { get; set; }
        public bool? DetermineBestOrientation { get; set; }
        public ImageResizeMode? ImageResizeMode { get; set; }

        public bool DoImageManipulation()
        {
            return DetermineBestOrientation.HasValue && ImageResizeMode.HasValue;
        }
    }
}