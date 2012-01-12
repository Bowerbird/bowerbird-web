using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;
using Raven.Client;

namespace Bowerbird.Core.Test.ProxyRepositories
{
    public class ProxyRepository<T> : IRepository<T>
    {

        #region Members

        protected IRepository<T> _repository;

        private Action<T> _onAdd;

        #endregion

        #region Constructors

        public ProxyRepository(IRepository<T> repository)
        {
            _repository = repository;
        }

        #endregion

        #region Properties

        public Raven.Client.IDocumentSession Session
        {
            get { return _repository.Session; }
        }

        #endregion

        #region Methods

        public T Load(string id)
        {
            return _repository.Load(id);
        }

        public IEnumerable<T> Load(IEnumerable<string> ids)
        {
            return _repository.Load(ids);
        }

        public void Add(T domainModel)
        {
            _repository.Add(domainModel);

            _onAdd(domainModel);
        }

        public void Add(IEnumerable<T> domainModels)
        {
            _repository.Add(domainModels);
        }

        public void Remove(T domainModel)
        {
            _repository.Remove(domainModel);
        }

        public void Remove(IEnumerable<T> domainModels)
        {
            _repository.Remove(domainModels);
        }

        public void SaveChanges()
        {
            _repository.SaveChanges();
        }

        public void NotifyOnAdd(Action<T> onAdd)
        {
            _onAdd = onAdd;
        }

        #endregion      
      
    }
}
