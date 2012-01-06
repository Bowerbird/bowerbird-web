using System;

namespace Bowerbird.Core.DomainModels
{
    /// <summary>
    ///     Facilitates indicating which property(s) describe the unique signature of an 
    ///     domainModel.  See DomainModel.GetTypeSpecificSignatureProperties() for when this is leveraged.
    /// </summary>
    /// <remarks>
    ///     This is intended for use with <see cref = "DomainModel" />.  It may NOT be used on a <see cref = "ValueObject" />.
    /// </remarks>
    //[Serializable]
    public class DomainSignatureAttribute : Attribute
    {
    }
} 