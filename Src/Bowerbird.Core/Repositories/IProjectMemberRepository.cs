using System.Collections.Generic;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Repositories
{
    public interface IProjectMemberRepository
    {
        ProjectMember Load(string projectId, string userId);

        ProjectMember Load(string id);

        IEnumerable<ProjectMember> Load(IEnumerable<string> ids);

        void Add(ProjectMember domainModel);

        void Remove(ProjectMember domainModel);

        void SaveChanges();
    }

}