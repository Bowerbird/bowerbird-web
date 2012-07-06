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
    public class AsynchActivity : DynamicObject
    {
        #region Members

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        #endregion

        #region Constructors

        protected AsynchActivity()
            : base()
        {
        }

        public AsynchActivity(
            string type,
            DateTime createdDateTime,
            object model,
            string clientId
            )
            : base()
        {
            Check.RequireNotNull(model, "model");

            // Add these properties to the dictionary. I tried making these properties static, but RavenDB has a bug where static properties on a
            // DynamicObject type are not serialised.
            _properties.Add("Type", type);
            _properties.Add("CreatedDateTime", createdDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            _properties.Add("CreatedDateTimeOrder", createdDateTime.ToString("yyyyMMddHHmmssfffffff"));
            _properties.Add("Model", model);
            _properties.Add("ClientId", clientId);
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