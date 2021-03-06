/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.DomainModelFactories
{
    public interface IMediaFilePathFactory
    {
        string MakeMediaUri(string uri);

        string MakeRelativeMediaFileUri(string mediaResourceId, string storedRepresentation, string extension);

        string MakeMediaFileName(string mediaResourceId, string storedRepresentation, string extension);

        string MakeMediaBasePath(string mediaResourceId);

        string MakeMediaFilePath(string mediaResourceId, string storedRepresentation, string extension);
    }
}