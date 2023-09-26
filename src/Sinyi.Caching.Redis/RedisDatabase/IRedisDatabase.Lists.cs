using StackExchange.Redis;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public partial interface IRedisDatabase
    {
        Task<long> ListAddToLeftAsync<T>(string key, T item, When when = When.Always, CommandFlags flag = CommandFlags.None)
            where T : class;

        long ListAddToLeft<T>(string key, T item, When when = When.Always, CommandFlags flag = CommandFlags.None)
            where T : class;

        Task<long> ListAddToLeftAsync<T>(string key, T[] items, CommandFlags flag = CommandFlags.None)
            where T : class;

        long ListAddToLeft<T>(string key, T[] items, CommandFlags flag = CommandFlags.None)
            where T : class;

        Task<T> ListGetFromRightAsync<T>(string key, CommandFlags flag = CommandFlags.None)
            where T : class;

        T ListGetFromRight<T>(string key, CommandFlags flag = CommandFlags.None)
            where T : class;
    }
}
