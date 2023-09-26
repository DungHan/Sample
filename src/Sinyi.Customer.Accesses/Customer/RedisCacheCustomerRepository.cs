using Microsoft.Extensions.Logging;
using Sinyi.Caching;
using Sinyi.Caching.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sinyi.Customers.Accesses
{
    public partial class RedisCacheCustomerRepository : CustomerRepository
    {
        // Fields
        private readonly CustomerRepository _customerRepository = null;

        private readonly IRedisDatabase _redisDatabase = null;

        private readonly ILogger<RedisCacheCustomerRepository> _logger = null;


        // Constructors
        public RedisCacheCustomerRepository(CustomerRepository customerRepository, 
                                            RedisCacheClient cache,
                                            ILogger<RedisCacheCustomerRepository> logger)
        {
            #region Contracts

            if (customerRepository == null) throw new ArgumentException(nameof(customerRepository));
            if (cache == null) throw new ArgumentException(nameof(cache));
            if (logger == null) throw new ArgumentException(nameof(logger));

            #endregion

            // Default
            _customerRepository = customerRepository;
            _redisDatabase = cache.GetDbFromConfiguration();
            _logger = logger;

            // Subscribe MQ
            SubscribeMQAsync().GetAwaiter();
        }


        // Methods
        public async Task<Customer> FindByCustomerIdAsync(string customerId, string agentId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(customerId) == true) throw new ArgumentException(nameof(customerId));

            #endregion

            // Get from cache or create
            var result = await _redisDatabase.GetOrCreateAsync(KeyGenerator.GetKey(this) + KeyGenerator.Join(customerId, agentId),
                                                               async () => await _customerRepository.FindByCustomerIdAsync(customerId, agentId),
                                                               TimeSpan.FromMinutes(30));

            // Return
            return result;
        }
    }
}
