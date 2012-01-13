using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Raven.Client;

namespace Bowerbird.Core.Repositories
{

    public static class ProjectMemberRepositoryExtensions
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static ProjectMember Load(this IRepository<ProjectMember> repository, string projectId, string userId)
        {
            string actualProjectId = "projects/" + projectId;
            string actualUserId = "users/" + userId;

            return repository
                .Session
                .Query<ProjectMember>()
                .Where(x => x.Project.Id == actualProjectId && x.User.Id == actualUserId)
                .FirstOrDefault();
        }

        #endregion

    }
}