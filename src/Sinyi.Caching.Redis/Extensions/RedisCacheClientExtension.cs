using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public static class RedisCacheClientExtension
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
        public static IEnumerable<TItem> GetOrCreateList<TInput, TItem>(this IRedisDatabase cache,
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
            var existItemAndnotExistsKeyList = GetItemOrCreateNotExistsKey<TInput, TItem>(cache, inputs, cacheKeySelector).GetAwaiter().GetResult();
            if (existItemAndnotExistsKeyList.itemList == null) throw new InvalidOperationException($"{nameof(existItemAndnotExistsKeyList.itemList)} is null");
            if (existItemAndnotExistsKeyList.notExistsKeyList == null) throw new InvalidOperationException($"{nameof(existItemAndnotExistsKeyList.notExistsKeyList)} is null");
            if (existItemAndnotExistsKeyList.notExistsKeyList.Count == 0) return existItemAndnotExistsKeyList.itemList;
            var existItemList = existItemAndnotExistsKeyList.itemList;
            var notExistsKeyList = existItemAndnotExistsKeyList.notExistsKeyList;

            // 處理 notExistsKeyList
            var remainingItemDic = itemFactory(notExistsKeyList).Where(r => itemKeySelector(r) != null)
                                                                .GroupBy(r => itemKeySelector(r))
                                                                .ToDictionary(gp => gp.Key, gp => gp.Select(r => r));
            if (remainingItemDic == null) throw new InvalidOperationException($"{nameof(remainingItemDic)} is null");

            // 將 remaining 記錄到 cache 內
            SetRemainingAsync(cache.Database, remainingItemDic, cacheKeySelector, absoluteExpirationRelativeToNow).GetAwaiter().GetResult();

            // 填入查不到 Item 的 key
            if (fillEmptyItem == true) FillEmptyAsync(cache.Database, notExistsKeyList, cacheKeySelector, remainingItemDic, absoluteExpirationRelativeToNow).GetAwaiter().GetResult();

            // Return
            return existItemList.Concat(remainingItemDic.SelectMany(r => r.Value));
        }

        public static async Task<IEnumerable<TItem>> GetOrCreateListAsync<TInput, TItem>(this IRedisDatabase cache,
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
            if (inputs.Count() == 0) return await Task.FromResult(new List<TItem>());

            // 取得在 Cache 裡的資料，與沒有的清單
            var existItemAndnotExistsKeyList = await GetItemOrCreateNotExistsKey<TInput, TItem>(cache, inputs, cacheKeySelector);
            if (existItemAndnotExistsKeyList.itemList == null) throw new InvalidOperationException($"{nameof(existItemAndnotExistsKeyList.itemList)} is null");
            if (existItemAndnotExistsKeyList.notExistsKeyList == null) throw new InvalidOperationException($"{nameof(existItemAndnotExistsKeyList.notExistsKeyList)} is null");
            if (existItemAndnotExistsKeyList.notExistsKeyList.Count == 0) return existItemAndnotExistsKeyList.itemList;
            var existItemList = existItemAndnotExistsKeyList.itemList;
            var notExistsKeyList = existItemAndnotExistsKeyList.notExistsKeyList;

            // 處理 notExistsKeyList
            var remainingItemList = await itemFactory(notExistsKeyList);
            var remainingItemDic = remainingItemList.Where(r => itemKeySelector(r) != null)
                                                    .GroupBy(r => itemKeySelector(r))
                                                    .ToDictionary(gp => gp.Key, gp => gp.Select(r => r));
            if (remainingItemDic == null) throw new InvalidOperationException($"{nameof(remainingItemDic)} is null");

            // 將 remaining 記錄到 cache 內
            SetRemainingAsync(cache.Database, remainingItemDic, cacheKeySelector, absoluteExpirationRelativeToNow).GetAwaiter().GetResult();

            // 填入查不到 Item 的 key
            if (fillEmptyItem == true) FillEmptyAsync(cache.Database, notExistsKeyList, cacheKeySelector, remainingItemDic, absoluteExpirationRelativeToNow).GetAwaiter().GetResult();

            // Return
            return existItemList.Concat(remainingItemDic.SelectMany(r => r.Value));
        }

        // 取得 Cache 裡的資料與建立 notExistsKeyList
        private static async Task<(List<TItem> itemList, List<TInput> notExistsKeyList)> GetItemOrCreateNotExistsKey<TInput, TItem>(IRedisDatabase cache,
                                                                                                                                    IEnumerable<TInput> inputs,
                                                                                                                                    Func<TInput, string> keySelector)
        {
            #region Contracts

            if (inputs == null) throw new ArgumentException(nameof(inputs));

            #endregion

            // Fields
            var index = 0;
            var existsItemList = new List<TItem>();
            var notExistsKeyList = new List<TInput>();

            // Get values
            var redisKeys = CreateRedisKeys(inputs, keySelector);
            var values = await cache.Database.StringGetAsync(redisKeys);

            // 取得在 Cache 裡的 Item，與建立不在的 key 清單
            foreach (var key in inputs)
            {
                if (values[index].HasValue == true)
                    existsItemList.AddRange(JsonExtension.Deserialize<IEnumerable<TItem>>(values[index], false));
                else
                    notExistsKeyList.Add(key);           // 紀錄不在記錄內的清單

                index++;
            }

            // Return
            return (existsItemList, notExistsKeyList);
        }

        private static async Task SetRemainingAsync<TInput, TItem>(IDatabase database,
                                                                   Dictionary<TInput, IEnumerable<TItem>> remainingItemDic,
                                                                   Func<TInput, string> keySelector,
                                                                   TimeSpan absoluteExpirationRelativeToNow)
        {
            #region Contracts

            if (database == null) throw new ArgumentException(nameof(database));
            if (remainingItemDic == null) throw new ArgumentException(nameof(remainingItemDic));
            if (keySelector == null) throw new ArgumentException(nameof(keySelector));

            #endregion

            // Fields
            var values = remainingItemDic.ToDictionary(r => keySelector(r.Key), r => r.Value.Serialize(false));

            // Set to cache
            await BatchStringSetAsync(database, values, absoluteExpirationRelativeToNow);
        }

        private static async Task FillEmptyAsync<TInput, TItem>(IDatabase database,
                                                                List<TInput> notExistsKeyList,
                                                                Func<TInput, string> keySelector,
                                                                Dictionary<TInput, IEnumerable<TItem>> remainingItemDic,
                                                                TimeSpan absoluteExpirationRelativeToNow)
        {
            #region Contracts

            if (database == null) throw new ArgumentException(nameof(database));
            if (notExistsKeyList == null) throw new ArgumentException(nameof(notExistsKeyList));
            if (keySelector == null) throw new ArgumentException(nameof(keySelector));
            if (remainingItemDic == null) throw new ArgumentException(nameof(remainingItemDic));

            #endregion

            // Fields
            var emptyItemKeyList = notExistsKeyList.Where(notExistsKey => remainingItemDic.Any(remaining => remaining.Key.Equals(notExistsKey)) != true);
            if (emptyItemKeyList.Count() == 0) return;
            var values = emptyItemKeyList.Distinct().ToDictionary(e => keySelector(e), e => "[]");

            // Set to cache
            await BatchStringSetAsync(database, values, absoluteExpirationRelativeToNow);
        }

        private static RedisKey[] CreateRedisKeys<TInput>(IEnumerable<TInput> inputs, Func<TInput, string> keySelector)
        {
            // Return
            return inputs.Select(key => new RedisKey(keySelector(key))).ToArray();
        }

        private static async Task BatchStringSetAsync(IDatabase database,
                                                      IEnumerable<KeyValuePair<string, string>> values,
                                                      TimeSpan absoluteExpirationRelativeToNow,
                                                      int batchSize = 10000)
        {
            #region Contracts

            if (database == null) throw new ArgumentException(nameof(database));
            if (values == null) throw new ArgumentException(nameof(values));

            #endregion

            // Requirement
            if (values.Count() == 0) return;

            // Fields
            var skipped = 0;
            IEnumerable<KeyValuePair<string, string>> segmentValues;

            // Execute
            while ((segmentValues = values.Skip(skipped).Take(batchSize)).Any())
            {
                await BatchExecuteAsync(database, batch =>
                {
                    return segmentValues.Select(s => batch.StringSetAsync(s.Key, s.Value, absoluteExpirationRelativeToNow)).ToList();
                });

                // Shift
                skipped += batchSize;
            }
        }

        private static async Task BatchExecuteAsync<TResult>(IDatabase database, Func<IBatch, List<Task<TResult>>> factory)
        {
            #region Contracts

            if (factory == null) throw new ArgumentException(nameof(factory));

            #endregion

            // Fields
            var batch = database.CreateBatch();

            // Execute
            var tasks = factory(batch);
            batch.Execute();
            await Task.WhenAll(tasks.ToArray());
        }
    }
}
