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
namespace Bowerbird.Core.Services
{
    public interface IMediaFilePathService
    {
        string MakeMediaFileUri(MediaResource mediaResource, string storedRepresentation);

        string MakeMediaFileUri(string mediaResourceId, string mediaType, string storedRepresentation, string extension);

        string MakeMediaFileName(MediaResource mediaResource, string storedRepresentation);

        string MakeMediaFileName(string mediaResourceId, string storedRepresentation, string extension);

        string MakeMediaBasePath(int mediaResourceId, string mediaType);

        string MakeMediaFilePath(string mediaResourceId, string mediaType, string storedRepresentation, string extension);
    }
}