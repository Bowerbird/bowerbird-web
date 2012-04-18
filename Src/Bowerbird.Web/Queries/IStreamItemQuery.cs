using System.Collections.Generic;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Core.Queries
{
    public interface IStreamItemQuery
    {
        PagedList<StreamItem> GetStreamItems(StreamItemListInput listInput, StreamSortInput sortInput);
    }
}