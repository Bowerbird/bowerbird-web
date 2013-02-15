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
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    //public class MediaResourceFile : DynamicObject
    //{
    //    protected Dictionary<string, object> _properties = new Dictionary<string, object>();

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
    //}

    public abstract class MediaResourceFile
    {
        public string Uri { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class OriginalImageMediaResourceFile : MediaResourceFile
    {
        public string MimeType { get; set; }
        public string Filename { get; set; }
        public string Size { get; set; }
        public string ExifData { get; set; }
        //public string Uri { get; set; }
        //public int Width { get; set; }
        //public int Height { get; set; }
    }

    public class OriginalVideoMediaResourceFile : MediaResourceFile
    {
        public string Provider { get; set; }
        public string VideoId { get; set; }
        public object ProviderData { get; set; }
    }

    public class OriginalAudioMediaResourceFile : MediaResourceFile
    {
        public string MimeType { get; set; }
    }

    public class DerivedMediaResourceFile : MediaResourceFile
    {
        public string Key { get; set; }
        //public string Uri { get; set; }
        //public int Width { get; set; }
        //public int Height { get; set; }
    }
}