using System.Diagnostics.CodeAnalysis;

namespace ITPortal.Extension.System.Collections.Generic
{
    /// <summary>
    /// 集合扩展方法
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 如果条件成立，添加项
        /// </summary>
        public static void AddIf<T>(this ICollection<T> collection, T value, bool flag)
        {
            Guard.NotNull(collection, nameof(collection));
            if (flag)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 如果条件成立，添加项
        /// </summary>
        public static void AddIf<T>(this ICollection<T> collection, T value, Func<bool> func)
        {
            Guard.NotNull(collection, nameof(collection));
            if (func())
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 获取对象，不存在对使用委托添加对象
        /// </summary>
        public static T GetOrAdd<T>(this ICollection<T> collection, Func<T, bool> selector, Func<T> factory)
        {
            Guard.NotNull(collection, nameof(collection));
            T item = collection.FirstOrDefault(selector);
            if (item == null)
            {
                item = factory();
                collection.Add(item);
            }

            return item;
        }

        /// <summary>
        /// 判断集合是否为null或空集合
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 交换两项的位置
        /// </summary>
        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            Guard.Between(index1, nameof(index1), 0, list.Count, true);
            Guard.Between(index2, nameof(index2), 0, list.Count, true);

            if (index1 == index2)
            {
                return;
            }

            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }
  

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, T item)
        {
            Check.NotNull(source, nameof(source));

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }

        /// <summary>
        /// Adds items to the collection which are not already in the collection.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="items">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns the added items.</returns>
        public static IEnumerable<T> AddIfNotContains<T>([NotNull] this ICollection<T> source, IEnumerable<T> items)
        {
            Check.NotNull(source, nameof(source));

            var addedItems = new List<T>();

            foreach (var item in items)
            {
                if (source.Contains(item))
                {
                    continue;
                }

                source.Add(item);
                addedItems.Add(item);
            }

            return addedItems;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection based on the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to decide if the item is already in the collection</param>
        /// <param name="itemFactory">A factory that returns the item</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, [NotNull] Func<T, bool> predicate, [NotNull] Func<T> itemFactory)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(itemFactory, nameof(itemFactory));

            if (source.Any(predicate))
            {
                return false;
            }

            source.Add(itemFactory());
            return true;
        }

        /// <summary>
        /// Removes all items from the collection those satisfy the given <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to remove the items</param>
        /// <returns>List of removed items</returns>
        public static IList<T> RemoveAll<T>([NotNull] this ICollection<T> source, Func<T, bool> predicate)
        {
            var items = source.Where(predicate).ToList();

            foreach (var item in items)
            {
                source.Remove(item);
            }

            return items;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <param name="source">The collection</param>
        /// <param name="items">Items to be removed from the list</param>
        public static void RemoveAll<T>([NotNull] this ICollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Remove(item);
            }
        }
    }
}