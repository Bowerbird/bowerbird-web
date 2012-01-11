using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {

        #region Members

        protected IDocumentSession _documentSession;

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

        public void Add(T domainModel)
        {
            _documentSession.Store(domainModel);
        }

        public void Remove(T domainModel)
        {
            _documentSession.Delete(domainModel);
        }

        public void SaveChanges()
        {
            _documentSession.SaveChanges();
        }

        #endregion      
     
    }
}
