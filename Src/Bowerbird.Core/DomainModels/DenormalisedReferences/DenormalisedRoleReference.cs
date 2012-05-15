///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Collections.Generic;
//using System.Linq;
//using Bowerbird.Core.DesignByContract;

//namespace Bowerbird.Core.DomainModels.DenormalisedReferences
//{
//    public class DenormalisedRoleReference : ValueObject
//    {
//        #region Members

//        #endregion

//        #region Constructors

//        #endregion

//        #region Properties

//        public string Id { get; private set; }

//        public string Name { get; private set; }

//        public IEnumerable<DenormalisedNamedDomainModelReference<Permission>> Permissions { get; private set; }

//        #endregion

//        #region Methods

//        public static implicit operator DenormalisedRoleReference(Role role)
//        {
//            Check.RequireNotNull(role, "role");

//            return new DenormalisedRoleReference
//            {
//                Id = role.Id,
//                Name = role.Name,
//                Permissions = role.Permissions
//            };
//        }

//        public IEnumerable<string> PermissionIds
//        {
//            get { return Permissions.Select(x => x.Id); }
//        }

//        #endregion
//    }
//}