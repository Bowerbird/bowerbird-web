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
    public interface IProjectsViewModelBuilder : IBuilder
    {
        object BuildProject(IdInput idInput);

        object BuildNewProject();

        object BuildProjectList(PagingInput pagingInput);

        object BuildUserProjectList(PagingInput pagingInput);

        object BuildTeamProjectList(PagingInput pagingInput);

        object BuildProjectUserList(PagingInput pagingInput);
    }
}