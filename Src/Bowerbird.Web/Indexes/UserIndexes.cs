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
    public class User_ByUserId : AbstractIndexCreationTask<User>
    {
        public User_ByUserId()
        {
            Map = users => users.Select(x => x.Id);
        }
    }

    public class User_ByEmail : AbstractIndexCreationTask<User>
    {
        public User_ByEmail()
        {
            Map = users => users.Select(x => x.Email);
        }
    }
}