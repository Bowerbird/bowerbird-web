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

        public static int GetEnumeratorCount<T>(this IEnumerable<T> collection)
        {
            var result = 0;

            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext()) result++;
            }

            return result;
        }

    }
}