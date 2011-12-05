using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : IEntity
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

        public TEntity Load(string id)
        {
            return _documentSession.Load<TEntity>(id);
        }

        public IEnumerable<TEntity> Load(IEnumerable<string> ids)
        {
            return _documentSession.Load<TEntity>(ids);
        }

        public void Add(TEntity entity)
        {
            _documentSession.Store(entity);
        }

        public void Remove(TEntity entity)
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
