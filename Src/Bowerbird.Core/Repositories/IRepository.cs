using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Repositories
{
    public interface IRepository<T>
    {
        //T Load(string id);

        //IEnumerable<T> Load(IEnumerable<string> ids);

        void Add(T domainModel);

        void Remove(T domainModel);

        void SaveChanges();

        //IList<T> GetAll();
        //T Save(T obj);
        //T Update(T obj);
        //void Delete(T obj);
        //T Get(string id);
        //T GetFirst();
        //T GetUnique(Func<T, bool> where);
        //IList<T> GetRestictedList(Func<T, bool> where);
        //void DeleteAll(IList<T> objs);
        //void UpdateAll(IList<T> objs);
        //void InsertAll(IList<T> objs);
  
    }
}
