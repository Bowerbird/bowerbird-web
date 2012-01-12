using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Repositories
{
    public interface IRepository<T>
    {
        T Load(string id);

        IEnumerable<T> Load(IEnumerable<string> ids);

        void Add(T domainModel);

        void Add(IEnumerable<T> domainModels);

        void Remove(T domainModel);

        void Remove(IEnumerable<T> domainModels);

        void SaveChanges();

        Raven.Client.IDocumentSession Session { get; }

        //IList<T> GetAll();
        //T GetFirst();
        //T GetUnique(Func<T, bool> where);
        //IList<T> GetRestictedList(Func<T, bool> where);
  
    }
}
