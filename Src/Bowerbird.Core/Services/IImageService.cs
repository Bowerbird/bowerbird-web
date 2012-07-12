/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System.Collections.Generic;
using System.IO;
using Bowerbird.Core.ImageUtilities;

namespace Bowerbird.Core.Services
{
    public interface IImageService
    {
        ImageService SaveAs(out Stream imageStream);

        ImageService SaveAs(string filename);
        
        ImageService Resize(ImageDimensions targetImageDimensions, bool determineBestOrientation, ImageResizeMode imageResizeMode);
        
        ImageService GetImageDimensions(out ImageDimensions imageDimensions);
        
        ImageService GetExifData(out IDictionary<string, object> exifData);
        
        void Cleanup();
        
        ImageService Reset();
    }
}