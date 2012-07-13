/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Core.Utilities
{
    ///
    /// Modes available for the resize method of images.
    ///
    public enum ImageResizeMode
    {
        ///
        /// This will resize images to the resolution nearest to the target resolution.
        ///
        Normal = 1,
        ///
        /// This will stretch an image so it always is the exact dimensions of the target resolution. Image may become distorted.
        ///
        Stretch = 2,
        ///
        /// This will size an image to the exact dimensions of the target resolution, keeping ratio in mind and cropping parts that can't
        /// fit within the dimensions of the target resolution.
        ///
        Crop = 3,
        ///
        /// This will size an image to the exact dimensions of the target resolution, keeping ratio in mind and filling up the image
        /// with white bars when some parts remain empty.
        ///
        Fill = 4,
        ///
        /// This will size an image to the fit inside the dimensions of the target resolution, keeping ratio in mind and filling up the image
        /// with white bars when vertical space remains empty.
        /// 
        FillVertical = 5
    }
}