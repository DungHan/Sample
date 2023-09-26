using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sinyi.Caching;
using Sinyi.Caching.Memory;
using Sinyi.Caching.Redis;
using Sinyi.Data.MsSqlClient;
using System;
using System.Collections.Generic;

namespace Sinyi.Customers
{
    public class CustomerContext
    {
        // Fields
        private readonly CustomerRepository _customerRepository = null;


        // Constructors
        public CustomerContext(CustomerRepository customerRepository)
        {
            #region Contracts

            if (customerRepository == null) throw new ArgumentException(nameof(customerRepository));

            #endregion

            // Default
            _customerRepository = customerRepository;
        }


        // Properties
        public CustomerRepository CustomerRepository => _customerRepository;
    }
}