using Sinyi.Caching;
using Sinyi.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinyi.Customers.Accesses
{
    public partial class MemoryCacheCustomerRepository : CustomerRepository
    {
        // Fields
        private CustomerRepository _customerRepository = null;

        private MemoryCacheClient _cache = null;


        // Constructors
        public MemoryCacheCustomerRepository(CustomerRepository customerRepository, 
                                             MemoryCacheClient cache)
        {
            #region Contracts

            if (customerRepository == null) throw new ArgumentException(nameof(customerRepository));
            if (cache == null) throw new ArgumentException(nameof(cache));

            #endregion

            // Default
            _customerRepository = customerRepository;
            _cache = cache;
        }


        // Methods
        public async Task<Customer> FindByCustomerIdAsync(string customerId, string agentId = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(customerId) == true) throw new ArgumentException(nameof(customerId));

            #endregion

            // Get from cache or create
            var result = await _cache.GetOrCreate(KeyGenerator.GetKey(this) + KeyGenerator.Join(customerId, agentId),
                                                  async () => await _customerRepository.FindByCustomerIdAsync(customerId, agentId),
                                                  TimeSpan.FromMinutes(1));

            // Return
            return result;
        }
    }
}
