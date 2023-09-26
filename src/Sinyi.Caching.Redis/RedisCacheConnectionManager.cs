using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public class RedisCacheConnectionManager
    {
        // Fields
        private readonly RedisConfig _config = null;

        private readonly Lazy<ConnectionMultiplexer> _lazyConnection = null;


        // Constructor
        public RedisCacheConnectionManager(RedisConfig config)
        {
            #region Contracts

            if (config == null) throw new ArgumentException(nameof(config));

            #endregion

            // Default
            _config = config;
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => Connect());
        }


        // Methods
        public IConnectionMultiplexer GetConnection()
        {
            // Return
            return _lazyConnection.Value;
        }

        private ConnectionMultiplexer Connect()
        {
            // Connect
            var connect = ConnectionMultiplexer.Connect(this.GetOptions(_config));

            // Event
            connect.ConnectionRestored += ConnectionRestored;
            connect.ConnectionFailed += ConnectionFailed;

            // Return
            return connect;
        }

        private ConfigurationOptions GetOptions(RedisConfig config)
        {
            // Get
            var connectionString = GetConnectionString();
            var options = ConfigurationOptions.Parse(connectionString);

            // Return
            return options;


            // SubFunction
            string GetConnectionString()
            {
                // Get
                if (string.IsNullOrEmpty(config.ConnectionString) != true)            // 優先使用環境變數
                    return config.ConnectionString;

                // Get server
                if (GetByConfig(out var connectionString) == false)                          // 再使用 config
                    throw new InvalidOperationException($"{nameof(config.Server)} is empty.");

                // Return
                return connectionString;
            }

            bool GetByConfig(out string connectionString)
            {
                // Fields
                var stringList = new List<string>();
                connectionString = string.Empty;

                // Create
                if (GetServerString(out var serverString) == true) stringList.Add(serverString);
                else return false;
                if (GetUserString(out var user) == true) stringList.Add(user);
                if (GetPassworeString(out var password) == true) stringList.Add(password);
                stringList.Add("abortConnect=false");

                // Set
                connectionString = string.Join(",", stringList);

                // Return
                return true;
            }

            bool GetServerString(out string serverString)
            {
                // Fields
                serverString = string.Empty;

                // Requirement
                if (string.IsNullOrEmpty(config.Server) == true) return false;

                // Get
                serverString = string.IsNullOrEmpty(config.Port) != true ? $"{config.Server}:{config.Port}" : $"{config.Server}";

                // Return
                return true;
            }

            bool GetUserString(out string user)
            {
                // Fields
                user = string.Empty;

                // Requirement
                if (string.IsNullOrEmpty(config.UserName) == true) return false;

                // Get
                user = $"user={config.UserName}";

                // Return
                return true;
            }

            bool GetPassworeString(out string password)
            {
                // Fields
                password = string.Empty;

                // Requirement
                if (string.IsNullOrEmpty(config.Password) == true) return false;

                // Get
                password = $"password={config.Password}";

                // Return
                return true;
            }
        }

        private void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            if (e.ConnectionType == ConnectionType.Subscription)
            {

            }
        }

        private void ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            if (e.ConnectionType == ConnectionType.Subscription)
            {

            }
        }
    }
}
