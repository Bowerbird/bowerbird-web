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
            _properties.Add("Id", "mediaresources/");
            _properties.Add("MediaType", mediaType);
            _properties.Add("UploadedOn", uploadedOn.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            _properties.Add("Metadata", new Dictionary<string, string>());

            if (mediaType != "image" && mediaType == "video" && mediaType == "audio" && mediaType == "document")
            {
                throw new ArgumentException(string.Format("The specified mediaType '{0}' is not recognised.", mediaType));
            }

            _properties.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mediaType), new Dictionary<string, MediaResourceFile>());

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

        private void AddFile(string mediaType, string storedRepresentation, MediaResourceFile file)
        {
            if (((IDictionary<string, MediaResourceFile>)_properties[mediaType]).ContainsKey(storedRepresentation))
            {
                ((IDictionary<string, MediaResourceFile>)_properties[mediaType]).Remove(storedRepresentation);
            }

            ((IDictionary<string, MediaResourceFile>)_properties[mediaType]).Add(storedRepresentation, file);
        }

        public MediaResourceFile AddImageFile(
            string storedRepresentation, 
            string filename, 
            string relativeUri, 
            string format, 
            int width, 
            int height, 
            string extension
            )
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
            string uri,
            string provider,
            string videoId,
            int width,
            int height
            )
        {
            dynamic file = new MediaResourceFile();

            file.Uri = uri;
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
            string documentType,
            string extension
            )
        {
            dynamic file = new MediaResourceFile();

            file.Name = fileName;
            file.DocumentType = documentType;
            file.Extension = extension;

            AddFile("Document", storedRepresentation, file);

            return file;
        }

        public MediaResourceFile AddAudioFile(
            string storedRepresentation,
            string fileName)
        {
            dynamic file = new MediaResourceFile();

            file.Name = fileName;
            file.Extension = "mp3";

            AddFile("Audio", storedRepresentation, file);

            return file;
        }

        public void FireCreatedEvent(User updatedByUser)
        {
            EventProcessor.Raise(new DomainModelCreatedEvent<MediaResource>(this, updatedByUser, this));
        }

        #endregion
    }
}