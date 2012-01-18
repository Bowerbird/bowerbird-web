using System;
using System.Collections.Generic;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModels.Shared;

namespace Bowerbird.Web.ViewModelFactories
{
    public interface IPagedListFactory
    {
        PagedList<T> Make<T>();

        PagedList<T> Make<T>(int page, int pageSize, int totalResultCount, IEnumerable<T> pageObjects, IDictionary<int, string> namedPages);
    }
}
