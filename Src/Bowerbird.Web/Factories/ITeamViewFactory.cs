/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;

namespace Bowerbird.Web.Factories
{
    public interface ITeamViewFactory : IFactory
    {
        object Make(Team team);

        object Make(Team team, PagedList<object> projects, object organisation, PagedList<object> members);
    }
}