/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Builders
{
    public interface ITeamsViewModelBuilder
    {
        object BuildTeam(IdInput idInput);

        object BuildTeamList(PagingInput pagingInput);

        object BuildUserTeamList(PagingInput pagingInput);

        object BuildOrganisationTeamList(PagingInput pagingInput);
    }
}