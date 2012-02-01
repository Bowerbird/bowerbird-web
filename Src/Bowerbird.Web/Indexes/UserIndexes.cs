/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client.Indexes;

namespace Bowerbird.Web.Indexes
{
    public class User_WithUserIdAndEmail : AbstractIndexCreationTask<User>
    {
        public User_WithUserIdAndEmail()
        {
            Map = users => from user in users
                           select new { UserId = user.Id, user.Email};
        }
    }
}