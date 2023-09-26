using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public class RedisConfig
    {
        // Properties
        public string Server { get; set; } = "127.0.0.1";

        public string Port { get; set; } = "6379";

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConnectionString { get; set; }

        public int DbIndex { get; set; } = 0;

        public string KeyPrefix { get; set; } = string.Empty;                                   // Key 前贅詞
    }
}
