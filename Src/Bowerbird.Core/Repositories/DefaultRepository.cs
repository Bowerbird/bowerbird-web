//using System.Collections.Generic;
//using Raven.Client;

//namespace Bowerbird.Core.Repositories
//{
    
//    public class DefaultRepository<T> : RepositoryBase<T>, IRepository<T>
//    {

//        #region Members

//        #endregion

//        #region Constructors

//        public DefaultRepository(IDocumentSession documentSession)
//            : base(documentSession)
//        {
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public T Load(string id)
//        {
//            return _documentSession.Load<T>(id);
//        }

//        public IEnumerable<T> Load(IEnumerable<string> ids)
//        {
//            return _documentSession.Load<T>(ids);
//        }

//        #endregion      
      
//    }
//}
