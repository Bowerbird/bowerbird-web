/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Services
{
    /// <summary>
    /// Please note that this factory DOES NOT have an implementation. Ninject creates a concrete class at initialisation.
    /// </summary>
    public interface IMediaServiceFactory
    {
        IAudioService CreateAudioService();

        IDocumentService CreateDocumentService();

        IImageService CreateImageService();

        IYouTubeVideoService CreateYouTubeVideoService();

        IVimeoVideoService CreateVimeoVideoService();
    }
}