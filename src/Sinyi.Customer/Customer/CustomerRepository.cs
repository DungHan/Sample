using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Customers
{
    // 取得客戶資料
    public interface CustomerRepository
    {
        // Methods
        Task<Customer> FindByCustomerIdAsync(string customerId, string agentId = null);
    }
}
