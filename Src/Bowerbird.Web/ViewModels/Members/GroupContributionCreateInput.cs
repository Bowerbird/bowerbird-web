/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System;

namespace Bowerbird.Web.ViewModels.Members
{
    public class GroupContributionCreateInput
    {
        public string UserId { get; set; }

        public string GroupId { get; set; }

        public string ContributionId { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
