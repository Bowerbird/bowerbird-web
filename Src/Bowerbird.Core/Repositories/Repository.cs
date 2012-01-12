using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Repositories
{
    public class Repository<T> : IRepository<T>
    {

        #region Members

        private IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public Repository(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        public IDocumentSession Session
        {
            get { return _documentSession; }
        }

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

        public void Add(T domainModel)
        {
            _documentSession.Store(domainModel);
        }

        public void Add(IEnumerable<T> domainModels)
        {
            foreach (var domainModel in domainModels)
            {
                Add(domainModel);
            }
        }

        public void Remove(T domainModel)
        {
            _documentSession.Delete(domainModel);
        }

        public void Remove(IEnumerable<T> domainModels)
        {
            foreach (var domainModel in domainModels)
            {
                Remove(domainModel);
            }
        }

        public void SaveChanges()
        {
            _documentSession.SaveChanges();
        }

        #endregion      
     
    }
}
