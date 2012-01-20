/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

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