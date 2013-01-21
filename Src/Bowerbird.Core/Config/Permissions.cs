/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Core.Config
{
    public static class PermissionNames
    {
        public static string CreateOrganisation = "createorganisation";
        public static string UpdateOrganisation = "updateorganisation";
        public static string DeleteOrganisation = "deleteorganisation";
        public static string CreateProject = "createproject";
        public static string UpdateProject = "updateproject";
        public static string DeleteProject = "deleteproject";
        //public static string CreateWatchlist = "createwatchlist";
        //public static string UpdateWatchlist = "updatewatchlist";
        //public static string DeleteWatchlist = "deletewatchlist";
        public static string CreateObservation = "createobservation";
        public static string UpdateObservation = "updateobservation";
        public static string DeleteObservation = "deleteobservation";
        public static string CreateSightingNote = "createsightingnote";
        public static string UpdateSightingNote = "updatesightingnote";
        public static string DeleteSightingNote = "deletesightingnote";
        public static string CreateIdentification = "createidentification";
        public static string UpdateIdentification = "updateidentification";
        public static string DeleteIdentification = "deleteidentification";
        public static string CreatePost = "createpost";
        public static string UpdatePost = "updatepost";
        public static string DeletePost = "deletepost";
        public static string CreateSpecies = "createspecies";
        public static string UpdateSpecies = "updatespecies";
        public static string DeleteSpecies = "deletespecices";
        public static string CreateComment = "createcomment";
        public static string UpdateComment = "updatecomment";
        public static string DeleteComment = "deletecomment";
        public static string LeaveProject = "leaveproject";
        public static string JoinOrganisation = "joinorganisation";
        public static string LeaveOrganisation = "leaveorganisation";
        public static string AddProject = "addproject";
        public static string RemoveProject = "removeproject";
        public static string Chat = "chat";
    }

    public static class RoleNames
    {
        /// <summary>
        /// value: globaladministrator
        /// </summary>
        public static string GlobalAdministrator = "globaladministrator";
        /// <summary>
        /// value: globalmember
        /// </summary>
        public static string GlobalMember = "globalmember";
        /// <summary>
        /// value: organisationadministrator
        /// </summary>
        public static string OrganisationAdministrator = "organisationadministrator";
        /// <summary>
        /// value: organsiationmember
        /// </summary>
        public static string OrganisationMember = "organisationmember";
        /// <summary>
        /// value: favouritesadministrator
        /// </summary>
        public static string FavouritesAdministrator = "favouritesadministrator";
        /// <summary>
        /// value: favouritesmember
        /// </summary>
        public static string FavouritesMember = "favouritesmember";
        /// <summary>
        /// value: projectadministrator
        /// </summary>
        public static string ProjectAdministrator = "projectadministrator";
        /// <summary>
        /// value: projectmember
        /// </summary>
        public static string ProjectMember = "projectmember";
        /// <summary>
        /// value: userprojectadministrator
        /// </summary>
        public static string UserProjectAdministrator = "userprojectadministrator";
        /// <summary>
        /// value: projectmember
        /// </summary>
        public static string UserProjectMember = "userprojectmember";
    }
}