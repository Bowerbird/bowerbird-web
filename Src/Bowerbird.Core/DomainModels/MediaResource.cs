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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

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
            string mediaType,
            User createdByUser,
            DateTime uploadedOn,
            string key)
            : base()
        {
            Check.RequireNotNullOrWhitespace(mediaType, "mediaType");
            //Check.RequireNotNullOrWhitespace(key, "key");
            //Check.RequireNotNull(createdByUser, "createdByUser");

            // Add these properties to the dictionary. I tried making these properties static, but RavenDB has a bug where static properties on a
            // DynamicObject type are not serialised.
            _properties.Add("Id", "");
            _properties.Add("MediaType", mediaType);
            _properties.Add("UploadedOn", uploadedOn.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            _properties.Add("Metadata", new Dictionary<string, string>());
            _properties.Add("Image", new Dictionary<string, MediaResourceFile>());
            _properties.Add("Video", new Dictionary<string, MediaResourceFile>());
            _properties.Add("Document", new Dictionary<string, MediaResourceFile>());
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
                _properties["Id"] = value;
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
            if (((IDictionary<string,object>)_properties["Metadata"]).ContainsKey(key))
            {
                ((IDictionary<string, object>)_properties["Metadata"])[key] = value;
            }
            else
            {
                ((IDictionary<string, object>)_properties["Metadata"]).Add(key, value);
            }

            return this;
        }

        private void AddFile(string mediaType, string storedRepresentation, MediaResourceFile file)
        {
            if (((IDictionary<string, MediaResourceFile>)_properties[mediaType]).ContainsKey(storedRepresentation))
            {
                ((IDictionary<string, MediaResourceFile>)_properties[mediaType]).Remove(storedRepresentation);
            }

            ((IDictionary<string, MediaResourceFile>)_properties[mediaType]).Add(storedRepresentation, file);
        }

        public MediaResourceFile AddImageFile(string storedRepresentation, string filename, string relativeUri, string format, int width, int height, string extension)
        {
            dynamic file = new MediaResourceFile();

            file.RelativeUri = relativeUri;
            file.Format = format;
            file.Width = width;
            file.Height = height;
            file.Extension = extension;

            AddFile("Image", storedRepresentation, file);

            return file;
        }

        public MediaResourceFile AddVideoFile(
            string storedRepresentation,
            string linkUri,
            string embedText,
            string provider,
            string videoId,
            string width,
            string height
            )
        {
            dynamic file = new MediaResourceFile();

            file.LinkUri = linkUri;
            file.EmbedTag = string.Format(embedText, width, height, videoId);
            file.Provider = provider;
            file.VideoId = videoId;
            file.Width = width;
            file.Height = height;

            AddFile("Video", storedRepresentation, file);

            return file;
        }

        public MediaResourceFile AddDocumentFile(
            string storedRepresentation,
            string fileName,
            string author,
            string documentType,
            string extension
            )
        {
            dynamic file = new MediaResourceFile();

            file.Name = fileName;
            file.Author = author;
            file.DocumentType = documentType;
            file.Extension = extension;

            AddFile("Document", storedRepresentation, file);

            return file;
        }

        public void FireCreatedEvent(User updatedByUser)
        {
            EventProcessor.Raise(new MediaResourceUploadedEvent(updatedByUser, this));
        }

        #endregion
    }
}