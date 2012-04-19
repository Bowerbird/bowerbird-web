using System.Collections.Generic;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Queries
{
    public interface IOrganisationsQuery
    {
        OrganisationIndex MakeOrganisationIndex(IdInput idInput);
        OrganisationList MakeOrganisationList(OrganisationListInput listInput);
        List<OrganisationView> GetGroupsHavingAddTeamPermission();
    }
}