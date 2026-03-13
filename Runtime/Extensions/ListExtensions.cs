using System.Collections.Generic;
using System.Linq;

namespace UnityUtils
{
    public static class ListExtensions
    {
        /// <summary>
        /// Determines whether a collection is null or has no elements
        /// without having to enumerate the entire collection to get a count.
        ///
        /// Uses LINQ's Any() method to determine if the collection is empty,
        /// so there is some GC overhead.
        /// </summary>
        /// <param name="list">List to evaluate</param>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || !list.Any();
        }

        /// <summary>
        /// Creates a new list that is a copy of the original list.
        /// </summary>
        /// <param name="list">The original list to be copied.</param>
        /// <returns>A new list that is a copy of the original list.</returns>
        public static List<T> Clone<T>(this IList<T> list)
        {
            List<T> newList = new List<T>();
            foreach (T item in list)
            {
                newList.Add(item);
            }

            return newList;
        }

        /// <summary>
        /// Swaps two elements in the list at the specified indices.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="indexA">The index of the first element.</param>
        /// <param name="indexB">The index of the second element.</param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }

        /// <summary>
        /// Shuffles the list in place.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.Swap(i, Helper.Rand.Next(i, list.Count));
            }
        }

        /// <summary>
        /// Gets a random item from the list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list from which to get a random item.</param>
        /// <returns>A random item from the list.</returns>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty())
            {
                return default;
            }

            return list[Helper.Rand.Next(0, list.Count)];
        }

        /// <summary>
        /// Gets a random item from the list and removes it from the list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list from which to get and remove a random item.</param>
        /// <param name="item">The random item that was selected and removed.</param>
        public static T GetRandomAndRemove<T>(this IList<T> list)
        {
            int index = Helper.Rand.Next(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
    }
}