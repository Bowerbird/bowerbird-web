using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.Test.ProxyRepositories
{
    public class ProxyProjectMemberRepository : ProxyRepository<ProjectMember>
    {

        #region Members

        #endregion

        #region Constructors

        public ProxyProjectMemberRepository(IRepository<ProjectMember> repository)
            : base(repository)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ProjectMember Load(string projectId, string userId)
        {
            return _repository.Load(projectId, userId);
        }

        #endregion      
      
    }
}
