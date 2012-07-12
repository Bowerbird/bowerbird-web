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

using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    /// <summary>
    ///     Provides a comparer for supporting LINQ methods such as Intersect, Union and Distinct.
    ///     This may be used for comparing objects of type <see cref = "BaseObject" /> and anything 
    ///     that derives from it, such as <see cref = "DomainModel" /> and <see cref = "ValueObject" />.
    /// 
    ///     NOTE:  Microsoft decided that set operators such as Intersect, Union and Distinct should 
    ///     not use the IEqualityComparer's Equals() method when comparing objects, but should instead 
    ///     use IEqualityComparer's GetHashCode() method.
    /// </summary>
    public class BaseObjectEqualityComparer<T> : IEqualityComparer<T>
        where T : DynamicBaseObject
    {
        public bool Equals(T firstObject, T secondObject)
        {
            // While SQL would return false for the following condition, returning true when 
            // comparing two null values is consistent with the C# language
            if (firstObject == null && secondObject == null)
            {
                return true;
            }

            if (firstObject == null ^ secondObject == null)
            {
                return false;
            }

            return firstObject.Equals(secondObject);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}