/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Builders
{
    public interface IStreamItemsViewModelBuilder
    {
        PagedList<object> BuildHomeStreamItems(StreamInput streamInput, PagingInput pagingInput);

        PagedList<object> BuildUserStreamItems(PagingInput pagingInput);

        PagedList<object> BuildGroupStreamItems(PagingInput pagingInput);
    }
}