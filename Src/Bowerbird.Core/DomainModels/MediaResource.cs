/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class MediaResource : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected MediaResource()
            : base()
        {
            InitMembers();

            EnableEvents();
        }

        public MediaResource(
            string type,
            User createdByUser,
            DateTime uploadedOn)
            : base()
        {
            InitMembers();

            Type = type;
            UploadedOn = uploadedOn;
            if (createdByUser != null)
            {
                CreatedByUser = createdByUser;
            }

            EnableEvents();
        }

        public MediaResource(
            string type,
            User createdByUser,
            DateTime uploadedOn,
            string description,
            string link,
            string provider,
            string videoId)
            : base()
        {
            InitMembers();

            Type = type;
            UploadedOn = uploadedOn;
            if (createdByUser != null)
            {
                CreatedByUser = createdByUser;
            }

            // Add metadata into the Metadata dictionary
            Metadata.Add("Description", description);
            Metadata.Add("Url", link);
            Metadata.Add("Provider", provider);
            Metadata.Add("VideoId", videoId);

            EnableEvents();
        }

        #endregion

        #region Properties

        public string Type { get; private set; }

        public DenormalisedUserReference CreatedByUser { get; private set; }

        public DateTime UploadedOn { get; private set; }

        public IDictionary<string, MediaResourceFile> Files { get; private set; }

        public IDictionary<string, string> Metadata { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Files = new Dictionary<string, MediaResourceFile>();
            Metadata = new Dictionary<string, string>();
        }

        public MediaResource AddMetadata(string key, string value)
        {
            if (Metadata.ContainsKey(key))
            {
                ((Dictionary<string, string>)Metadata)[key] = value;
            }
            else
            {
                ((Dictionary<string, string>)Metadata).Add(key, value);
            }

            return this;
        }

        private void AddFile(string storedRepresentation, MediaResourceFile file)
        {
            if (Files.ContainsKey(storedRepresentation))
            {
                ((Dictionary<string, MediaResourceFile>)Files).Remove(storedRepresentation);
            }

            ((Dictionary<string, MediaResourceFile>)Files).Add(storedRepresentation, file);
        }

        public MediaResourceFile AddImageFile(string storedRepresentation, string filename, string relativeUri, string format, int width, int height, string extension)
        {
            dynamic file = new MediaResourceFile();

            file.RelativeUri = relativeUri;
            file.Format = format;
            file.Width = width;
            file.Height = height;
            file.Extension = extension;

            AddFile(storedRepresentation, file);

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

            AddFile(storedRepresentation, file);

            return file;
        }

        public void FireCreatedEvent(User createdByUser)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            FireEvent(new DomainModelCreatedEvent<MediaResource>(this, createdByUser, this));            
        }

        #endregion      
    }

    public class MediaResourceFile : DynamicObject
    {
        protected Dictionary<string, object> _properties = new Dictionary<string, object>();

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
    }
}