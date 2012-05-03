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
        object BuildItem(IdInput idInput);

        object BuildIndex(PagingInput pagingInput);

        object BuildList(PagingInput pagingInput);
    }
}