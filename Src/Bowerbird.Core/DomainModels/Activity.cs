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
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Newtonsoft.Json;
using System.Dynamic;
using System.Collections;
using Raven.Abstractions.Linq;
using Raven.Json.Linq;

namespace Bowerbird.Core.DomainModels
{
    public class Activity : DynamicObject
    {
        #region Members

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        #endregion

        #region Constructors

        protected Activity()
        {
        }

        public Activity(
            string type,
            DateTime createdDateTime,
            string description,
            dynamic createdByUser,
            IEnumerable<dynamic> groups)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(groups, "groups");
            Check.Require(groups.Count() > 0, "at least one group must be specified");

            // Add these properties to the dictionary. I tried making these properties static, but RavenDB has a bug where static properties on a
            // DynamicObject type are not serialised.
            _properties.Add("Type", type);
            _properties.Add("CreatedDateTime", createdDateTime);
            _properties.Add("Description", description);
            _properties.Add("User", createdByUser);
            _properties.Add("Groups", groups);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _properties.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            dynamic method = _properties[binder.Name];
            result = method(args);
            return true;
        }

        #endregion
    }
}