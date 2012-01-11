using System.Collections.Generic;

namespace Bowerbird.Core.Repositories
{
    public interface IDefaultRepository<T> : IRepository<T>
    {

        T Load(string id);

        IEnumerable<T> Load(IEnumerable<string> ids);

    }
}