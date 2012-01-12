//using System.Collections.Generic;
//using System.Linq;
//using Bowerbird.Core.DomainModels;
//using Raven.Client;

//namespace Bowerbird.Core.Repositories
//{
    
//    public class ProjectMemberRepository : RepositoryBase<ProjectMember>, IProjectMemberRepository
//    {
//        public ProjectMemberRepository(IDocumentSession documentSession)
//            : base(documentSession)
//        {
//        }

//        public ProjectMember Load(string projectId, string userId)
//        {
//            return _documentSession
//                .Load<ProjectMember>()
//                .Where(x => x.Project.Id == projectId && x.User.Id == userId)
//                .FirstOrDefault();
//        }

//        public ProjectMember Load(string id)
//        {
//            return _documentSession.Load<ProjectMember>(id);
//        }

//        public IEnumerable<ProjectMember> Load(IEnumerable<string> ids)
//        {
//            return _documentSession.Load<ProjectMember>(ids);
//        }
//    }
//}
