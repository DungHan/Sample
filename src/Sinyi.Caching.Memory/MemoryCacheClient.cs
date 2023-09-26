using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sinyi.Caching.Memory
{
    public class MemoryCacheClient : MemoryCache
    {
        // Fields
        private IMemoryCache _memoryCache = null;


        // Constructor
        public MemoryCacheClient(IMemoryCache memoryCache)
        {
            #region Contracts

            if (memoryCache == null) throw new ArgumentException(nameof(memoryCache));

            #endregion

            // Default
            _memoryCache = memoryCache;
        }


        // Methods
        public TItem Set<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow, bool compression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Fields
            TItem result = default(TItem);

            // Set
            if (compression == true)
                result = this.SetCompression(key, value, absoluteExpirationRelativeToNow);
            else
                result = _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);

            // Return
            return result;
        }

        public bool Remove(string key)
        {
            #region Contracts

            if (key == null) throw new ArgumentException(nameof(key));

            #endregion

            // Remove
            _memoryCache.Remove(key);

            // Return
            return true;
        }

        public bool Remove(IEnumerable<string> keyList)
        {
            #region Contracts

            if (keyList == null) throw new ArgumentException(nameof(keyList));

            #endregion

            // Remove
            foreach(var key in keyList)
                _memoryCache.Remove(key);

            // Return
            return true;
        }

        public bool RemoveAll(string pattern)
        {
            #region Contracts

            if (pattern == null) throw new ArgumentException(nameof(pattern));

            #endregion

            // Remove
            _memoryCache.RemoveAllByStartsWithKey(pattern);

            // Return
            return true;
        }

        public TItem Get<TItem>(string key, bool decompression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Fields
            TItem result = default(TItem);

            // Get
            if (decompression == true)
                result = this.GetDecompression<TItem>(key);
            else
                result = _memoryCache.Get<TItem>(key);

            // Return
            return result;
        }

        public bool TryGetValue<TItem>(string key, out TItem value, bool decompression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Fields
            var result = false;

            // Get
            if (decompression == true)
            {
                value = this.GetDecompression<TItem>(key);
                result = value != null;
            }
            else
                result = _memoryCache.TryGetValue(key, out value);

            // Return
            return result;
        }

        public TItem GetOrCreate<TItem>(string key, Func<TItem> factory, TimeSpan absoluteExpirationRelativeToNow, bool decompressionOrcompression = false)
        {
            #region Contracts

            if (key == null) throw new ArgumentException(nameof(key));
            if (factory == null) throw new ArgumentException(nameof(factory));

            #endregion

            // Fields
            TItem result = default(TItem);

            // Get
            if (decompressionOrcompression == true)
                result = this.GetOrCreateCompression(key, factory, absoluteExpirationRelativeToNow);
            else
            {
                result = _memoryCache.GetOrCreate(key, cacheEntry =>
                                                  {
                                                      cacheEntry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
                                                      return factory();
                                                  });
            }
           
            // Return
            return result;
        }

        public bool Contains(string key)
        {
            #region Contracts

            if (key == null) throw new ArgumentException(nameof(key));

            #endregion

            // Get
            var result = _memoryCache.TryGetValue(key, out _);

            // Return
            return result;
        }

        private TItem SetCompression<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Compression
            var byteCompression = Compress(value);

            // Set
            _memoryCache.Set(key, byteCompression, absoluteExpirationRelativeToNow);

            // Return
            return value;
        }

        private TItem GetDecompression<TItem>(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Fields
            TItem result = default(TItem);

            // Get
            if (_memoryCache.TryGetValue(key, out string value) == true)
                result = Decompress<TItem>(value);

            // Return
            return result;
        }

        private TItem GetOrCreateCompression<TItem>(string key, Func<TItem> factory, TimeSpan absoluteExpirationRelativeToNow)
        {
            #region Contracts

            if (key == null) throw new ArgumentException(nameof(key));
            if (factory == null) throw new ArgumentException(nameof(factory));

            #endregion

            // Get from cache or create
            var cacheValue = _memoryCache.GetOrCreate(key, cacheEntry =>
            {
                // Get
                cacheEntry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;

                // Run
                var result = factory();

                // Return
                return this.Compress(result);
            });

            // Decompression
            var result = Decompress<TItem>(cacheValue);

            // Return
            return result;
        }

        private string Compress(object item)
        {
            #region Contracts

            if (item == null) throw new ArgumentException(nameof(item));

            #endregion

            // Convert
            var json = JsonSerializer.Serialize(item);
            var result = StringCompress.Compress(json);

            // Return
            return result;
        }

        private T Decompress<T>(string serializedObject)
        {
            #region Contracts

            if (serializedObject == null) throw new ArgumentException(nameof(serializedObject));

            #endregion

            try
            {
                // Decompression
                serializedObject = StringCompress.Decompress(serializedObject);

                // Deserialize
                var result = JsonSerializer.Deserialize<T>(serializedObject);

                // Return
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
