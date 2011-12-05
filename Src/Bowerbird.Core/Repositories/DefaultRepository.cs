using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Entities;
using Raven.Client;

namespace Bowerbird.Core.Repositories
{
    public class DefaultRepository<TEntity> : RepositoryBase<TEntity> where TEntity : IEntity
    {

        #region Members

        #endregion

        #region Constructors

        public DefaultRepository(IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion      
      
    }
}
