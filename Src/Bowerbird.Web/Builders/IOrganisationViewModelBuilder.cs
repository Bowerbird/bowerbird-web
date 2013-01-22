/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Organisation Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Builders
{
    public interface IOrganisationViewModelBuilder : IBuilder
    {
        object BuildCreateOrganisation();

        object BuildUpdateOrganisation(string organisationId);

        dynamic BuildOrganisation(string organisationId);

        object BuildOrganisationList(OrganisationsQueryInput organisationsQueryInput);

        object BuildUserOrganisationList(string userId, PagingInput pagingInput);
    }
}