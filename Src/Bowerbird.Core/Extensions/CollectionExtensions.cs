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

namespace Bowerbird.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNotNullAndHasItems(this IEnumerable<object> collection)
        {
            return collection != null && collection.Count() > 0;
        }

        public static List<T> AddRangeFromList<T>(this List<T> collection, List<T> add)
        {
            if (collection != null && add != null)
            {
                collection.AddRange(add);
            }

            return collection;
        }

        public static IEnumerable<T> ExtractFromResults<T>(this IEnumerable<T> results, Func<IEnumerable<T>, List<string>> subQuery, out List<string> ids)
        {
            ids = null;

            if(results != null)
            {
                ids = subQuery(results);
            }

            return results;
        }
    }
}