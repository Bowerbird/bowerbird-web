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
    public abstract class MediaResource : DomainModel
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private IDictionary<string, string> _metadata;

        #endregion

        #region Constructors

        protected MediaResource()
        {
            InitMembers();
        }

        public MediaResource(
            string mediaResourceType,
            User createdByUser,
            DateTime uploadedOn,
            string key,
            IDictionary<string, string> metadata)
            : base()
        {
            Check.RequireNotNullOrWhitespace(mediaResourceType, "mediaResourceType");
            Check.RequireNotNullOrWhitespace(key, "key");
            //Check.RequireNotNull(metadata, "metadata");

            InitMembers();

            Id = "mediaresources/";
            MediaResourceType = mediaResourceType;
            UploadedOn = uploadedOn;
            if(metadata != null) _metadata = metadata;
            if(createdByUser != null) CreatedByUser = createdByUser;
            Key = key;
        }

        #endregion

        #region Properties

        public string Key { get; set; }

        public DenormalisedUserReference CreatedByUser { get; set; }

        public string MediaResourceType { get; set; }

        public DateTime UploadedOn { get; set; }

        public IDictionary<string, string> Metadata 
        {
            get { return _metadata; }
            set { _metadata = value; }
        } 

        #endregion

        #region Methods

        private void InitMembers()
        {
            _metadata = new Dictionary<string, string>();
        }

        public MediaResource AddMetadata(string key, string value)
        {
            if (_metadata.ContainsKey(key))
            {
                _metadata[key] = value;
            }
            else
            {
                _metadata.Add(key, value);
            }

            return this;
        }

        protected MediaResourceFile FileByMediaType(string mediaType)
        {
            switch (mediaType.ToLower())
            {
                case "image":
                    return new OriginalImageMediaResourceFile();
                case "video":
                    return new OriginalVideoMediaResourceFile();
                case "audio":
                    return new OriginalAudioMediaResourceFile();
                default:
                    return new DerivedMediaResourceFile();
            }
        }

        #endregion
    }

    public class ImageMediaResource : MediaResource
    {
        protected ImageMediaResource()
            : base()
        {
            InitMembers();
        }

        public ImageMediaResource(
            string mediaResourceType,
            User createdByUser,
            DateTime uploadedOn,
            string key,
            IDictionary<string, string> metadata)
            : base(
            mediaResourceType,
            createdByUser,
            uploadedOn,
            key,
            metadata)
        {
            InitMembers();
        }

        private void InitMembers()
        {
            Image = new InternalImageMediaResource();
        }

        public InternalImageMediaResource Image { get; set; }
    }

    public class InternalImageMediaResource
    {
        public OriginalImageMediaResourceFile Original { get; set; }
        public DerivedMediaResourceFile Square50 { get; set; }
        public DerivedMediaResourceFile Square100 { get; set; }
        public DerivedMediaResourceFile Square200 { get; set; }
        public DerivedMediaResourceFile Constrained240 { get; set; }
        public DerivedMediaResourceFile Constrained480 { get; set; }
        public DerivedMediaResourceFile Constrained600 { get; set; }
        public DerivedMediaResourceFile Full640 { get; set; }
        public DerivedMediaResourceFile Full800 { get; set; }
        public DerivedMediaResourceFile Full1024 { get; set; }
        public DerivedMediaResourceFile Small { get; set; }
        public DerivedMediaResourceFile Large { get; set; }
    }

    public class VideoMediaResource : MediaResource
    {
        protected VideoMediaResource()
            : base()
        {
            InitMembers();
        }

        public VideoMediaResource(
            string mediaResourceType,
            User createdByUser,
            DateTime uploadedOn,
            string key,
            IDictionary<string, string> metadata)
            : base(
            mediaResourceType,
            createdByUser,
            uploadedOn,
            key,
            metadata)
        {
            InitMembers();
        }

        private void InitMembers()
        {
            Video = new InternalVideoMediaResource();
        }

        public InternalVideoMediaResource Video { get; set; }
    }

    public class InternalVideoMediaResource
    {
        public OriginalVideoMediaResourceFile Original { get; set; }
        public OriginalImageMediaResourceFile OriginalImage { get; set; }
        public DerivedMediaResourceFile Square50 { get; set; }
        public DerivedMediaResourceFile Square100 { get; set; }
        public DerivedMediaResourceFile Square200 { get; set; }
        public DerivedMediaResourceFile Constrained240 { get; set; }
        public DerivedMediaResourceFile Constrained480 { get; set; }
        public DerivedMediaResourceFile Constrained600 { get; set; }
        public DerivedMediaResourceFile Full640 { get; set; }
        public DerivedMediaResourceFile Full800 { get; set; }
        public DerivedMediaResourceFile Full1024 { get; set; }
    }

    public class AudioMediaResource : MediaResource
    {
        protected AudioMediaResource()
            : base()
        {
            InitMembers();
        }

        public AudioMediaResource(
            string mediaResourceType,
            User createdByUser,
            DateTime uploadedOn,
            string key,
            IDictionary<string, string> metadata)
            : base(
            mediaResourceType,
            createdByUser,
            uploadedOn,
            key,
            metadata)
        {
            InitMembers();
        }

        private void InitMembers()
        {
            Audio = new InternalAudioMediaResource();
        }

        public InternalAudioMediaResource Audio { get; set; }
    }

    public class InternalAudioMediaResource
    {
        public OriginalAudioMediaResourceFile Original { get; set; }
        public DerivedMediaResourceFile Square50 { get; set; }
        public DerivedMediaResourceFile Square100 { get; set; }
        public DerivedMediaResourceFile Square200 { get; set; }
        public DerivedMediaResourceFile Constrained240 { get; set; }
        public DerivedMediaResourceFile Constrained480 { get; set; }
        public DerivedMediaResourceFile Constrained600 { get; set; }
        public DerivedMediaResourceFile Full640 { get; set; }
        public DerivedMediaResourceFile Full800 { get; set; }
        public DerivedMediaResourceFile Full1024 { get; set; }
    }

    //public class MediaResource : DynamicObject
    //{
    //    #region Members

    //    private Dictionary<string, object> _properties = new Dictionary<string, object>();

    //    #endregion

    //    #region Constructors

    //    protected MediaResource()
    //    {
    //    }

    //    public MediaResource(
    //        string mediaResourceType,
    //        User createdByUser,
    //        DateTime uploadedOn,
    //        string key,
    //        Dictionary<string, string> metadata)
    //        : base()
    //    {
    //        Check.RequireNotNullOrWhitespace(mediaResourceType, "mediaResourceType");
    //        Check.RequireNotNullOrWhitespace(key, "key");
    //        //Check.RequireNotNull(createdByUser, "createdByUser");
    //        Check.RequireNotNull(metadata, "metadata");

    //        if (mediaResourceType != Constants.MediaResourceTypes.Image && 
    //            mediaResourceType == Constants.MediaResourceTypes.Video && 
    //            mediaResourceType == Constants.MediaResourceTypes.Audio &&
    //            mediaResourceType == Constants.MediaResourceTypes.Document)
    //        {
    //            throw new ArgumentException(string.Format("The specified mediaResourceType '{0}' is not recognised.", mediaResourceType));
    //        }

    //        // Add these properties to the dictionary. I tried making these properties static, but RavenDB has a bug where static properties on a
    //        // DynamicObject type are not serialised.
    //        _properties.Add("Id", "mediaresources/");
    //        _properties.Add("MediaResourceType", mediaResourceType);
    //        _properties.Add("UploadedOn", uploadedOn.ToString("yyyy-MM-ddTHH:mm:ssZ"));
    //        _properties.Add("Metadata", metadata);

    //        _properties.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mediaResourceType), new Dictionary<string, MediaResourceFile>());

    //        if(createdByUser != null) _properties.Add("User", (DenormalisedUserReference)createdByUser);

    //        if(!string.IsNullOrEmpty(key)) _properties.Add("Key", key);
    //    }

    //    #endregion

    //    #region Properties

    //    public string Id
    //    {
    //        get
    //        {
    //            return _properties["Id"].ToString();
    //        }
    //        set
    //        {
    //            if (!_properties.ContainsKey("Id"))
    //            {
    //                _properties.Add("Id", string.Empty);
    //            }
    //            _properties["Id"] = value;
    //        }
    //    }

    //    public string Key
    //    {
    //        get 
    //        { 
    //            return _properties["Key"].ToString();
    //        }
    //        set
    //        {
    //            if(!_properties.ContainsKey("Key"))
    //            {
    //                _properties.Add("Key", string.Empty);
    //            }
    //            _properties["Key"] = value;
    //        }
    //    }

    //    public string MediaResourceType
    //    {
    //        get
    //        {
    //            return _properties["MediaResourceType"].ToString();
    //        }
    //    }

    //    public IDictionary<string, string> Metadata
    //    {
    //        get
    //        {
    //            return (IDictionary<string, string>)_properties["Metadata"];
    //        }
    //        set
    //        {
    //            if (!_properties.ContainsKey("Metadata"))
    //            {
    //                _properties.Add("Metadata", new Dictionary<string, string>());
    //            }
    //            _properties["Metadata"] = value;
    //        }
    //    }

    //    #endregion

    //    #region Methods

    //    public override IEnumerable<string> GetDynamicMemberNames()
    //    {
    //        return _properties.Keys;
    //    }

    //    public override bool TryGetMember(GetMemberBinder binder, out object result)
    //    {
    //        return _properties.TryGetValue(binder.Name, out result);
    //    }

    //    public override bool TrySetMember(SetMemberBinder binder, object value)
    //    {
    //        _properties[binder.Name] = value;
    //        return true;
    //    }

    //    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    //    {
    //        dynamic method = _properties[binder.Name];
    //        result = method(args);
    //        return true;
    //    }

    //    public MediaResource AddMetadata(string key, string value)
    //    {
    //        if (((IDictionary<string,string>)_properties["Metadata"]).ContainsKey(key))
    //        {
    //            ((IDictionary<string, string>)_properties["Metadata"])[key] = value;
    //        }
    //        else
    //        {
    //            ((IDictionary<string, string>)_properties["Metadata"]).Add(key, value);
    //        }

    //        return this;
    //    }

    //    public MediaResourceFile AddFile(
    //        string storedRepresentation,
    //        string uri,
    //        int width,
    //        int height)
    //    {
    //        dynamic file = new MediaResourceFile();

    //        file.Uri = uri;
    //        file.Width = width;
    //        file.Height = height;

    //        var field = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_properties["MediaResourceType"].ToString());
    //        if (((IDictionary<string, MediaResourceFile>)_properties[field]).ContainsKey(storedRepresentation))
    //        {
    //            ((IDictionary<string, MediaResourceFile>)_properties[field]).Remove(storedRepresentation);
    //        }

    //        ((IDictionary<string, MediaResourceFile>)_properties[field]).Add(storedRepresentation, file);

    //        return file;
    //    }

    //    //public MediaResourceFile AddDerivedFile(
    //    //    string storedRepresentation,
    //    //    string uri,
    //    //    int width,
    //    //    int height)
    //    //{
    //    //    var file = new DerivedMediaResourceFile()
    //    //        {
    //    //            Uri = uri,
    //    //            Width = width,
    //    //            Height = height
    //    //        };

    //    //    var field = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_properties["MediaResourceType"].ToString());
    //    //    if (((IDictionary<string, MediaResourceFile>)_properties[field]).ContainsKey(storedRepresentation))
    //    //    {
    //    //        ((IDictionary<string, MediaResourceFile>)_properties[field]).Remove(storedRepresentation);
    //    //    }

    //    //    ((IDictionary<string, MediaResourceFile>)_properties[field]).Add(storedRepresentation, file);

    //    //    return file;
    //    //}

    //    //public MediaResourceFile AddOriginalImageFile(
    //    //    string mimeType,
    //    //    string filename,
    //    //    string size,
    //    //    string exifData,
    //    //    string uri)
    //    //{
    //    //    var file = new OriginalImageMediaResourceFile()
    //    //    {
    //    //        MimeType = mimeType,
    //    //        Filename = filename,
    //    //        Size = size,
    //    //        ExifData = exifData,
    //    //        Uri = uri
    //    //    };

    //    //    var field = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_properties["MediaResourceType"].ToString());
    //    //    if (((IDictionary<string, MediaResourceFile>)_properties[field]).ContainsKey("Original"))
    //    //    {
    //    //        ((IDictionary<string, MediaResourceFile>)_properties[field]).Remove("Original");
    //    //    }

    //    //    ((IDictionary<string, MediaResourceFile>)_properties[field]).Add("Original", file);

    //    //    return file;
    //    //}

    //    //public MediaResourceFile AddOriginalVideoFile(
    //    //    string provider,
    //    //    string videoId,
    //    //    object providerData)
    //    //{
    //    //    var file = new OriginalVideoMediaResourceFile()
    //    //    {
    //    //        Provider = provider,
    //    //        VideoId = videoId,
    //    //        ProviderData = providerData
    //    //    };

    //    //    var field = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_properties["MediaResourceType"].ToString());
    //    //    if (((IDictionary<string, MediaResourceFile>)_properties[field]).ContainsKey("Original"))
    //    //    {
    //    //        ((IDictionary<string, MediaResourceFile>)_properties[field]).Remove("Original");
    //    //    }

    //    //    ((IDictionary<string, MediaResourceFile>)_properties[field]).Add("Original", file);

    //    //    return file;
    //    //}

    //    //public MediaResourceFile AddOriginalAudioFile(
    //    //    string mimeType)
    //    //{
    //    //    var file = new OriginalAudioMediaResourceFile()
    //    //    {
    //    //        MimeType = mimeType,
    //    //    };

    //    //    var field = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_properties["MediaResourceType"].ToString());
    //    //    if (((IDictionary<string, MediaResourceFile>)_properties[field]).ContainsKey("Original"))
    //    //    {
    //    //        ((IDictionary<string, MediaResourceFile>)_properties[field]).Remove("Original");
    //    //    }

    //    //    ((IDictionary<string, MediaResourceFile>)_properties[field]).Add("Original", file);

    //    //    return file;
    //    //}

    //    #endregion
    //}
}