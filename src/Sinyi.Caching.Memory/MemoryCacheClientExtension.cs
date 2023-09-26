using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Memory
{
    public static class MemoryCacheClientExtension
    {
        /// <summary>
        /// GetOrCreate List 版。
        /// 若要求的資料只有部分在 cache 內，會呼叫 itemFactory 取得剩餘資訊再存入 cache
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="cache"></param>
        /// <param name="inputs"></param>
        /// <param name="cacheKeySelector">產生 cacheKey 的委派</param>
        /// <param name="itemFactory">取得 inputs 的方法</param>
        /// <param name="itemKeySelector">從 itemFactory 取得 input 值的委派</param>
        /// <param name="absoluteExpirationRelativeToNow">Cache 清除時間</param>
        /// <param name="fillEmptyItem">Input 找不到時，插入一筆空白資料避免下次觸發重載</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IEnumerable<TItem> GetOrCreateList<TInput, TItem>(this MemoryCache cache,
                                                                        IEnumerable<TInput> inputs,
                                                                        Func<TInput, string> cacheKeySelector,
                                                                        Func<List<TInput>, IEnumerable<TItem>> itemFactory,
                                                                        Func<TItem, TInput> itemKeySelector,
                                                                        TimeSpan absoluteExpirationRelativeToNow,
                                                                        bool fillEmptyItem = false)
        {
            #region Contracts

            if (cache == null) throw new ArgumentException(nameof(cache));
            if (inputs == null) throw new ArgumentException(nameof(inputs));
            if (cacheKeySelector == null) throw new ArgumentException(nameof(cacheKeySelector));
            if (itemFactory == null) throw new ArgumentException(nameof(itemFactory));
            if (itemKeySelector == null) throw new ArgumentException(nameof(itemKeySelector));

            #endregion

            // Requirement
            if (inputs.Count() == 0) return new List<TItem>();

            // 取得在 Cache 裡的資料，與沒有的清單
            var existItemList = GetItems<TInput, TItem>(cache, inputs, cacheKeySelector, out var notExistsKeyList);
            if (existItemList == null) throw new InvalidOperationException($"{nameof(existItemList)} is null");
            if (notExistsKeyList == null) throw new InvalidOperationException($"{nameof(notExistsKeyList)} is null");
            if (notExistsKeyList.Count == 0) return existItemList;

            // 處理 notExistsKeyList
            var remainingItemDic = itemFactory(notExistsKeyList).Where(r => itemKeySelector(r) != null)
                                                                .GroupBy(r => itemKeySelector(r))
                                                                .ToDictionary(gp => gp.Key, gp => gp.Select(r => r));
            if (remainingItemDic == null) throw new InvalidOperationException($"{nameof(remainingItemDic)} is null");

            // 逐筆記錄到 cache 內
            foreach (var item in remainingItemDic)
                cache.Set(cacheKeySelector(item.Key), item.Value, absoluteExpirationRelativeToNow);                            // 使用 keySelector 轉換 key

            // 填入查不到 Item 的 key
            if (fillEmptyItem == true) FillEmpty(cache, notExistsKeyList, cacheKeySelector, remainingItemDic, absoluteExpirationRelativeToNow);

            // Return
            return existItemList.Concat(remainingItemDic.SelectMany(r => r.Value));
        }

        public static async Task<IEnumerable<TItem>> GetOrCreateListAsync<TInput, TItem>(this MemoryCache cache,
                                                                                         IEnumerable<TInput> inputs,
                                                                                         Func<TInput, string> cacheKeySelector,
                                                                                         Func<List<TInput>, Task<IEnumerable<TItem>>> itemFactory,
                                                                                         Func<TItem, TInput> itemKeySelector,
                                                                                         TimeSpan absoluteExpirationRelativeToNow,
                                                                                         bool fillEmptyItem = false)
        {
            #region Contracts

            if (cache == null) throw new ArgumentException(nameof(cache));
            if (inputs == null) throw new ArgumentException(nameof(inputs));
            if (cacheKeySelector == null) throw new ArgumentException(nameof(cacheKeySelector));
            if (itemFactory == null) throw new ArgumentException(nameof(itemFactory));
            if (itemKeySelector == null) throw new ArgumentException(nameof(itemKeySelector));

            #endregion

            // Requirement
            if (inputs.Count() == 0) return new List<TItem>();

            // 取得在 Cache 裡的資料，與沒有的清單
            var existItemList = GetItems<TInput, TItem>(cache, inputs, cacheKeySelector, out var notExistsKeyList);
            if (existItemList == null) throw new InvalidOperationException($"{nameof(existItemList)} is null");
            if (notExistsKeyList == null) throw new InvalidOperationException($"{nameof(notExistsKeyList)} is null");
            if (notExistsKeyList.Count == 0) return existItemList;

            // 處理 notExistsKeyList
            var notExistsItemList = await itemFactory(notExistsKeyList);
            var remainingItemDic = notExistsItemList.Where(r => itemKeySelector(r) != null)
                                                    .GroupBy(r => itemKeySelector(r))
                                                    .ToDictionary(gp => gp.Key, gp => gp.Select(r => r));
            if (remainingItemDic == null) throw new InvalidOperationException($"{nameof(remainingItemDic)} is null");

            // 逐筆記錄到 cache 內
            foreach (var item in remainingItemDic)
                cache.Set(cacheKeySelector(item.Key), item.Value, absoluteExpirationRelativeToNow);                            // 使用 keySelector 轉換 key

            // 填入查不到 Item 的 key
            if (fillEmptyItem == true) FillEmpty(cache, notExistsKeyList, cacheKeySelector, remainingItemDic, absoluteExpirationRelativeToNow);

            // Return
            return existItemList.Concat(remainingItemDic.SelectMany(r => r.Value));
        }

        // 取得 Cache 裡的資料與建立 notExistsKeyList
        private static List<TItem> GetItems<TInput, TItem>(MemoryCache cache,
                                                           IEnumerable<TInput> intpus,
                                                           Func<TInput, string> keySelector,
                                                           out List<TInput> notExistsKeyList)
        {
            #region Contracts

            if (intpus == null) throw new ArgumentException(nameof(intpus));

            #endregion

            // Fields
            notExistsKeyList = new List<TInput>();
            var existsItemList = new List<TItem>();

            // 取得在 Cache 裡的 Item，與建立不在的 key 清單
            foreach (var key in intpus)
            {
                if (cache.TryGetValue(keySelector(key), out IEnumerable<TItem> item) == true)
                {
                    if (item != null) existsItemList.AddRange(item);
                }
                else
                    notExistsKeyList.Add(key);           // 紀錄不在記錄內的清單
            }

            // Return
            return existsItemList;
        }

        private static void FillEmpty<TInput, TItem>(MemoryCache cache,
                                                     List<TInput> notExistsKeyList,
                                                     Func<TInput, string> cacheKeySelector,
                                                     Dictionary<TInput, IEnumerable<TItem>> remainingItemDic,
                                                     TimeSpan absoluteExpirationRelativeToNow)
        {
            #region Contracts

            if (cache == null) throw new ArgumentException(nameof(cache));
            if (notExistsKeyList == null) throw new ArgumentException(nameof(notExistsKeyList));
            if (cacheKeySelector == null) throw new ArgumentException(nameof(cacheKeySelector));
            if (remainingItemDic == null) throw new ArgumentException(nameof(remainingItemDic));

            #endregion

            // Fields
            var emptyItemKeyList = notExistsKeyList.Where(notExistsKey => remainingItemDic.Any(remaining => remaining.Key.Equals(notExistsKey)) != true);
            
            // Set to cache
            foreach (var emptyItemKey in emptyItemKeyList)
                cache.Set(cacheKeySelector(emptyItemKey), default(TItem), absoluteExpirationRelativeToNow);
        }

        public static void RemoveAllByStartsWithKey(this IMemoryCache memoryCache, string startsWithKey)
        {
            #region Contracts

            if (memoryCache == null) throw new ArgumentException(nameof(memoryCache));
            if (string.IsNullOrEmpty(startsWithKey) == true) throw new ArgumentException(nameof(startsWithKey));

            #endregion

            // Get
            var allkeys = memoryCache.GetAllKeys(startsWithKey);

            // Remove all
            allkeys.ForEach(key => memoryCache.Remove(key));
        }

        public static List<string> GetAllKeys(this IMemoryCache memoryCache)
        {
            // Fields
            var resultList = new List<string>();

            // Reflection
            var field = memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(memoryCache) as ICollection;
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    resultList.Add(val.ToString());
                }
            }

            // Return
            return resultList;
        }

        public static List<string> GetAllKeys(this IMemoryCache memoryCache, string startsWithKey)
        {
            #region Contracts

            if (memoryCache == null) throw new ArgumentException(nameof(memoryCache));
            if (string.IsNullOrEmpty(startsWithKey) == true) throw new ArgumentException(nameof(startsWithKey));

            #endregion

            // Get
            var keys = memoryCache.GetAllKeys();
            var allkeys = keys.FindAll(k => k.StartsWith(startsWithKey));

            // Reture
            return allkeys;
        }
    }
}
