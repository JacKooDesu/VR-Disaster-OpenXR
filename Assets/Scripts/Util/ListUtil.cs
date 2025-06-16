using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class ListUtil
    {
        public static T Random<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T Random<T>(this IList<T> list, int min, int max)
        {
            min = min < 0 ? 0 : min;
            max = max >= list.Count ? list.Count : max;
            return list[UnityEngine.Random.Range(min, max)];
        }

        public static int GetMaxIndex(this IList<int> list)
        {
            int temp = int.MinValue;
            int index = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] > temp)
                {
                    temp = list[i];
                    index = i;
                }
            }

            return index;
        }

        public static IList<T> Randomize<T>(this IList<T> list)
        {
            var sorted = list.OrderBy(t => System.Guid.NewGuid()).ToList();
            return sorted;
        }
    }
}