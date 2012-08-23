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
using System.Dynamic;

namespace Bowerbird.Core.DomainModels
{
    public class Activity : DynamicObject
    {
        #region Members

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        #endregion

        #region Constructors

        protected Activity()
            : base()
        {
        }

        public Activity(
            string type,
            DateTime createdDateTime,
            string description,
            dynamic createdByUser,
            IEnumerable<dynamic> groups,
            string contributionId = (string)null)
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(groups, "groups");
            Check.Require(groups.Count() > 0, "at least one group must be specified");

            // Add these properties to the dictionary. I tried making these properties static, but RavenDB has a bug where static properties on a
            // DynamicObject type are not serialised.
            _properties.Add("Type", type);
            _properties.Add("CreatedDateTime", createdDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            _properties.Add("CreatedDateTimeOrder", createdDateTime.ToString("yyyyMMddHHmmss"));
            _properties.Add("Description", description);
            _properties.Add("User", createdByUser);
            _properties.Add("Groups", groups);

            // this property is used for posting comments on Posts and Observations
            // so on the front end, it is possible to apply a new comment to the view
            // which renders the contribution
            if (contributionId != null)
            {
                _properties.Add("ContributionId", contributionId);
            }
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

        #endregion
    }
}