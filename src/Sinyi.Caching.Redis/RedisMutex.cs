using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public static class RedisMutex
    {
        // 自旋鎖，如果執行緒沒有獲取鎖，則會進入迴圈直到獲得上鎖的資格
        public static void SpinLock(this IDatabase database, string lockKey, Action action, int retryTime = 1000, int retryDelayMs = 10, int lockExpiryS = 10)
        {
            #region Contracts

            if (database == null) throw new ArgumentException(nameof(database));
            if (string.IsNullOrEmpty(lockKey) == true) throw new ArgumentException(nameof(lockKey));
            if (action == null) throw new ArgumentException(nameof(action));

            #endregion

            // Fields
            var token = Environment.MachineName;

            try
            {
                if (Lock() == true)
                    action();
            }
            finally
            {
                database.LockRelease(SetKeyPrefix(lockKey), token);
            }


            // SubFunction
            bool Lock()
            {
                // Fields
                var result = false;
                var count = 0;

                // Get
                do
                {
                    if (database.LockTake(SetKeyPrefix(lockKey), token, TimeSpan.FromSeconds(lockExpiryS)) == true)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        Task.Delay(retryDelayMs).Wait();
                    }
                }
                while(count++ < retryTime);
                if (!result) throw new InvalidOperationException($"Fail to take lock {lockKey}");
                
                // Return
                return result;
            }
        }

        public static async Task SpinLockAsync(this IDatabase database, string lockKey, Func<Task> action,  int retryTime = 1000, int retryDelayMs = 10, int lockExpiryS = 10)
        {
            #region Contracts

            if (database == null) throw new ArgumentException(nameof(database));
            if (string.IsNullOrEmpty(lockKey) == true) throw new ArgumentException(nameof(lockKey));
            if (action == null) throw new ArgumentException(nameof(action));

            #endregion

            // Fields
            var token = Environment.MachineName;

            try
            {
                if (await Lock() == true)
                    await action();
            }
            finally
            {
                await database.LockReleaseAsync(SetKeyPrefix(lockKey), token);
            }


            // SubFunction
            async Task<bool> Lock()
            {
                // Fields
                var result = false;
                var count = 0;

                // Get
                do
                {
                    if (await database.LockTakeAsync(SetKeyPrefix(lockKey), token, TimeSpan.FromSeconds(lockExpiryS)) == true)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        await Task.Delay(retryDelayMs);
                    }
                }
                while (count++ < retryTime);
                if (!result) throw new InvalidOperationException($"Fail to take lock {lockKey}");

                // Return
                return result;
            }
        }

        public static void Lock(this IDatabase database, string lockKey, Action action, int lockExpiryS = 10)
        {
            #region Contracts

            if (database == null) throw new ArgumentException(nameof(database));
            if (string.IsNullOrEmpty(lockKey) == true) throw new ArgumentException(nameof(lockKey));
            if (action == null) throw new ArgumentException(nameof(action));

            #endregion

            // Fields
            var token = Environment.MachineName;

            try
            {
                if (Lock() == true)
                    action();
            }
            finally
            {
                database.LockRelease(SetKeyPrefix(lockKey), token);
            }


            // SubFunction
            bool Lock()
            {
                // Return
                return database.LockTake(SetKeyPrefix(lockKey), token, TimeSpan.FromSeconds(lockExpiryS));
            }
        }

        public static async Task LockAsync(this IDatabase database, string lockKey, Func<Task> action, int lockExpiryS = 10)
        {
            #region Contracts

            if (database == null) throw new ArgumentException(nameof(database));
            if (string.IsNullOrEmpty(lockKey) == true) throw new ArgumentException(nameof(lockKey));
            if (action == null) throw new ArgumentException(nameof(action));

            #endregion

            // Fields
            var token = Environment.MachineName;

            try
            {
                if (await Lock() == true)
                    await action();
            }
            finally
            {
                await database.LockReleaseAsync(SetKeyPrefix(lockKey), token);
            }


            // SubFunction
            async Task<bool> Lock()
            {
                // Return
                return await database.LockTakeAsync(SetKeyPrefix(lockKey), token, TimeSpan.FromSeconds(lockExpiryS));
            }
        }

        private static string SetKeyPrefix(string key) => $"Lock:{key}";
    }
}
