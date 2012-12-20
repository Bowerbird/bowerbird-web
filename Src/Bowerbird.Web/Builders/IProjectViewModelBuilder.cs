/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Builders
{
    public interface IProjectViewModelBuilder : IBuilder
    {
        object BuildCreateProject();

        object BuildUpdateProject(string projectId);

        dynamic BuildProject(string projectId);

        object BuildProjectList(ProjectsQueryInput projectsQueryInput);

        object BuildUserProjectList(string userId, PagingInput pagingInput);
    }
}