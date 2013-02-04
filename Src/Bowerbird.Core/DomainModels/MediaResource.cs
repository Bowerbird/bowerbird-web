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
using System.Dynamic;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System.Globalization;

namespace Bowerbird.Core.DomainModels
{
    public class MediaResource : DynamicObject
    {
        #region Members

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        #endregion

        #region Constructors

        protected MediaResource()
        {
        }

        public MediaResource(
            string mediaResourceType,
            User createdByUser,
            DateTime uploadedOn,
            string key,
            Dictionary<string, string> metadata)
            : base()
        {
            Check.RequireNotNullOrWhitespace(mediaResourceType, "mediaResourceType");
            Check.RequireNotNullOrWhitespace(key, "key");
            //Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(metadata, "metadata");

            if (mediaResourceType != Constants.MediaResourceTypes.Image && 
                mediaResourceType == Constants.MediaResourceTypes.Video && 
                mediaResourceType == Constants.MediaResourceTypes.Audio &&
                mediaResourceType == Constants.MediaResourceTypes.Document)
            {
                throw new ArgumentException(string.Format("The specified mediaResourceType '{0}' is not recognised.", mediaResourceType));
            }

            // Add these properties to the dictionary. I tried making these properties static, but RavenDB has a bug where static properties on a
            // DynamicObject type are not serialised.
            _properties.Add("Id", "mediaresources/");
            _properties.Add("MediaResourceType", mediaResourceType);
            _properties.Add("UploadedOn", uploadedOn.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            _properties.Add("Metadata", metadata);

            _properties.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mediaResourceType), new Dictionary<string, MediaResourceFile>());

            if(createdByUser != null) _properties.Add("User", (DenormalisedUserReference)createdByUser);

            if(!string.IsNullOrEmpty(key)) _properties.Add("Key", key);
        }

        #endregion

        #region Properties

        public string Id
        {
            get
            {
                return _properties["Id"].ToString();
            }
            set
            {
                if (!_properties.ContainsKey("Id"))
                {
                    _properties.Add("Id", string.Empty);
                }
                _properties["Id"] = value;
            }
        }

        public string Key
        {
            get 
            { 
                return _properties["Key"].ToString();
            }
            set
            {
                if(!_properties.ContainsKey("Key"))
                {
                    _properties.Add("Key", string.Empty);
                }
                _properties["Key"] = value;
            }
        }

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

        public MediaResource AddMetadata(string key, string value)
        {
            if (((IDictionary<string,string>)_properties["Metadata"]).ContainsKey(key))
            {
                ((IDictionary<string, string>)_properties["Metadata"])[key] = value;
            }
            else
            {
                ((IDictionary<string, string>)_properties["Metadata"]).Add(key, value);
            }

            return this;
        }

        public MediaResourceFile AddFile(
            string storedRepresentation,
            string uri,
            int width,
            int height)
        {
            dynamic file = new MediaResourceFile();

            file.Uri = uri;
            file.Width = width;
            file.Height = height;

            var field = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_properties["MediaResourceType"].ToString());
            if (((IDictionary<string, MediaResourceFile>)_properties[field]).ContainsKey(storedRepresentation))
            {
                ((IDictionary<string, MediaResourceFile>)_properties[field]).Remove(storedRepresentation);
            }

            ((IDictionary<string, MediaResourceFile>)_properties[field]).Add(storedRepresentation, file);

            return file;
        }

        #endregion
    }
}