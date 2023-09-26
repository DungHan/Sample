using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public partial interface IRedisDatabase
    {
        Task<bool> HashDeleteAsync(string key, string hashField, CommandFlags flag = CommandFlags.None);

        bool HashDelete(string key, string hashField, CommandFlags flag = CommandFlags.None);

        Task<long> HashDeleteAsync(string key, IEnumerable<string> hashFields, CommandFlags flag = CommandFlags.None);

        long HashDelete(string key, IEnumerable<string> hashFields, CommandFlags flag = CommandFlags.None);

        Task<bool> HashExistsAsync(string key, string hashField, CommandFlags flag = CommandFlags.None);

        bool HashExists(string key, string hashField, CommandFlags flag = CommandFlags.None);

        Task<T> HashGetAsync<T>(string key, string hashField, CommandFlags flag = CommandFlags.None);
        
        T HashGet<T>(string key, string hashField, CommandFlags flag = CommandFlags.None);

        Task<Dictionary<string, T>> HashGetAsync<T>(string key, IList<string> hashFields, CommandFlags flag = CommandFlags.None);

        Dictionary<string, T> HashGet<T>(string key, IList<string> hashFields, CommandFlags flag = CommandFlags.None);

        Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, CommandFlags flag = CommandFlags.None);

        Dictionary<string, T> HashGetAll<T>(string key, CommandFlags flag = CommandFlags.None);

        Task<long> HashIncerementByAsync(string key, string hashField, long value = 1, CommandFlags flag = CommandFlags.None);

        long HashIncerement(string key, string hashField, long value = 1, CommandFlags flag = CommandFlags.None);

        Task<double> HashIncerementByAsync(string key, string hashField, double value = 1, CommandFlags flag = CommandFlags.None);

        double HashIncerement(string key, string hashField, double value = 1, CommandFlags flag = CommandFlags.None);

        Task<IEnumerable<string>> HashKeysAsync(string key, CommandFlags flag = CommandFlags.None);

        IEnumerable<string> HashKeys(string key, CommandFlags flag = CommandFlags.None);

        Task<long> HashLengthAsync(string key, CommandFlags flag = CommandFlags.None);

        long HashLength(string key, CommandFlags flag = CommandFlags.None);

        Task<bool> HashSetAsync<T>(string key, string hashField, T value, When when = When.Always, CommandFlags flag = CommandFlags.None);

        bool HashSet<T>(string key, string hashField, T value, When when = When.Always, CommandFlags flag = CommandFlags.None);

        Task HashSetAsync<T>(string key, IDictionary<string, T> values, CommandFlags flag = CommandFlags.None);

        void HashSet<T>(string key, IDictionary<string, T> values, CommandFlags flag = CommandFlags.None);

        Task<IEnumerable<T>> HashValuesAsync<T>(string key, CommandFlags flag = CommandFlags.None);

        IEnumerable<T> HashValues<T>(string key, CommandFlags flag = CommandFlags.None);

        Dictionary<string, T> HashScan<T>(string key, string pattern, int pageSize = 10, CommandFlags flag = CommandFlags.None);
    }
}
