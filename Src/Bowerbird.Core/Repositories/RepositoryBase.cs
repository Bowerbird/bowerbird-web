using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {

        #region Members

        private IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public RepositoryBase(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public T Load(string id)
        {
            return _documentSession.Load<T>(id);
        }

        public IEnumerable<T> Load(IEnumerable<string> ids)
        {
            return _documentSession.Load<T>(ids);
        }

        public void Add(T entity)
        {
            _documentSession.Store(entity);
        }

        public void Remove(T entity)
        {
            _documentSession.Delete(entity);
        }

        public void SaveChanges()
        {
            _documentSession.SaveChanges();
        }

        #endregion      
     
    }
}
