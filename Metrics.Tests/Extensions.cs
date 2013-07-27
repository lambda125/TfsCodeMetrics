using System.Collections.Generic;
using System.Linq;

namespace Metrics.Tests
{
    public static class Extensions
    {
        public static bool IsIn<T>(this T item, IEnumerable<T> list)
        {
            if (list == null)
                return false;

            var listArray = list.ToArray();
            if (!listArray.Any())
                return false;

            return listArray.Contains(item);
        }

        public static bool IsNotNull(this object something)
        {
            return (something != null);
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }
    }
}
