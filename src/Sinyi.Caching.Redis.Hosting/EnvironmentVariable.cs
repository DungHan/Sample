using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis.Hosting
{
    public class EnvironmentVariable
    {
        // Properties
        public string ConnectionString { get; set; } = string.Empty;     // 連線字串

        public string KeyPrefix { get; set; } = string.Empty;            // 前贅詞
    }
}
