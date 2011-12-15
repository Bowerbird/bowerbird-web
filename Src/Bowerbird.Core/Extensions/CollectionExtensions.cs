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

    }
}