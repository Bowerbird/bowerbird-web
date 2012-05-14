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

namespace Bowerbird.Core.Services
{
    public interface IConfigService : IService
    {
        string GetEnvironmentRootPath();

        string GetEnvironmentRootUri();

        string GetEmailServerName();

        string GetMediaRootUri();

        string GetMediaRelativePath();

        string GetDatabaseUrl();

        string GetDatabaseName();

        string GetEmailAdminAccount();

        string GetEmailResetPasswordRelativeUri();

        string GetSpeciesRelativePath();
    }
}