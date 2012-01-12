//using System.Collections.Generic;
//using System.Linq;
//using Bowerbird.Core.DomainModels;
//using Raven.Client;

//namespace Bowerbird.Core.Repositories
//{
    
//    public class UserRepository : RepositoryBase<User>, IRepository<User>
//    {
//        public UserRepository(IDocumentSession documentSession)
//            : base(documentSession)
//        {
//        }

//        public User Load(string id)
//        {
//            return _documentSession.Load<User>(id);
//        }

//        public User LoadByEmail(string email)
//        {
//            return _documentSession
//                .Load<User>()
//                .Where(x => x.Email == email)
//                .FirstOrDefault();
//        }
//    }
//}