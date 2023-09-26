using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Sinyi.MessageQueue.MQTTnet;

namespace Sinyi.Customers.Services
{
    [Authorize]
    [MDP.AspNetCore.Module("Sinyi-Customers")]
    public partial class CustomerController : Controller
    {
        // Fields
        private readonly CustomerContext _customerContext = null;

        private readonly MQTTnetClientFactory _mQTTnetClientFactory = null;


        // Constructors
        public CustomerController(CustomerContext customerContext, MQTTnetClientFactory mQTTnetClientFactory)
        {
            #region Contracts

            if (customerContext == null) throw new ArgumentException(nameof(customerContext));
            if (mQTTnetClientFactory == null) throw new ArgumentException(nameof(mQTTnetClientFactory));

            #endregion

            // Default
            _customerContext = customerContext;
            _mQTTnetClientFactory = mQTTnetClientFactory;
        }


        // Properties
        internal string CustomerMqTopic => MQTTKeyGenerator.Join("Sinyi.Customers.Accesses", nameof(CustomerRepository), "RemoveCache");

        internal string CustomerBriefMqTopic => MQTTKeyGenerator.Join("Sinyi.Customers.Accesses", nameof(CustomerBriefRepository), "RemoveCache");


        // Methods
        private bool VerifyIdentity(string verifyAgentId)
        {
            // Requiremen
            if (string.IsNullOrEmpty(verifyAgentId) == true) return false;

            // Get
            var loginAgentId = (this.User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(loginAgentId) == true) return false;

            // Verify
            return verifyAgentId == loginAgentId;
        }


        //Class
        public class FindAllCustomerResultModel
        {
            //Constructor
            public FindAllCustomerResultModel()
            {
                this.CustomerList = new List<Customer>();
            }

            // Properties
            public List<Customer> CustomerList { get; set; }             //公司的客戶
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<AddResultModel>> Add([FromBody] AddActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Add
            var result = await _customerContext.CustomerRepository.AddAsync(actionModel.Customer);

            // Remove cache
            var mqttPublisher = _mQTTnetClientFactory.CreatePublisher();
            await mqttPublisher.PublishAsync($"{CustomerMqTopic}/AgentId", result.AgentId);
            await mqttPublisher.PublishAsync($"{CustomerBriefMqTopic}/AgentId", result.AgentId);

            //Return
            return new AddResultModel()
            {
                Customer = result
            };
        }


        // Class
        public class AddActionModel
        {
            // Properties
            public Customer Customer { get; set; } = new Customer();
        }

        public class AddResultModel
        {
            // Properties
            public Customer Customer { get; set; } = new Customer();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<UpdateResultModel>> Update([FromBody] UpdateActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (actionModel.Customer == null) throw new InvalidOperationException($"{nameof(actionModel.Customer)} is empty");
            if (this.VerifyIdentity(actionModel.Customer.AgentId) == false) throw new InvalidOperationException($"Cannot update other agent's customer. This customer belongs to {actionModel.Customer.AgentId}");

            // Update
            var result = await _customerContext.CustomerRepository.UpdateAsync(actionModel.Customer);

            // Remove cache
            var mqttPublisher = _mQTTnetClientFactory.CreatePublisher();
            await mqttPublisher.PublishAsync($"{CustomerMqTopic}/AgentId", result.AgentId);
            await mqttPublisher.PublishAsync($"{CustomerMqTopic}/CustomerId", result.CustomerId);
            await mqttPublisher.PublishAsync($"{CustomerBriefMqTopic}/AgentId", result.AgentId);
            await mqttPublisher.PublishAsync($"{CustomerBriefMqTopic}/CustomerId", result.CustomerId);

            //Return
            return new UpdateResultModel()
            {
                Customer = result
            };
        }


        // Class
        public class UpdateActionModel
        {
            // Properties
            public Customer Customer { get; set; } = new Customer();
        }

        public class UpdateResultModel
        {
            // Properties
            public Customer Customer { get; set; } = new Customer();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<RemoveResultModel>> Remove([FromBody] RemoveActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (actionModel.CustomerId == null) throw new InvalidOperationException($"{nameof(actionModel.CustomerId)} is empty");

            // Remove
            var result = await _customerContext.CustomerRepository.RemoveAsync(actionModel.CustomerId);

            // Remove cache
            var mqttPublisher = _mQTTnetClientFactory.CreatePublisher();
            await mqttPublisher.PublishAsync($"{CustomerMqTopic}/CustomerId", actionModel.CustomerId);
            await mqttPublisher.PublishAsync($"{CustomerBriefMqTopic}/CustomerId", actionModel.CustomerId);

            //Return
            return new RemoveResultModel()
            {
                Respone = result
            };
        }


        // Class
        public class RemoveActionModel
        {
            // Properties
            public string CustomerId { get; set; }
        }

        public class RemoveResultModel
        {
            // Properties
            public bool Respone { get; set; }
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<FindAllByCustomerIdListResultModel>> FindAllByCustomerIdList([FromBody] FindAllByCustomerIdListActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (actionModel.CustomerIdList == null) throw new InvalidOperationException($"{nameof(actionModel.CustomerIdList)} is empty");

            // Find
            var result = await _customerContext.CustomerRepository.FindAllByCustomerIdListAsync(actionModel.CustomerIdList, actionModel.AgentId);

            // Return
            return new FindAllByCustomerIdListResultModel()
            {
                CustomerList = result
            };
        }


        // Class
        public class FindAllByCustomerIdListActionModel
        {
            // Properties
            public List<string> CustomerIdList { get; set; } = new List<string>();

            public string AgentId { get; set; }
        }

        public class FindAllByCustomerIdListResultModel
        {
            // Properties
            public List<Customer> CustomerList { get; set; } = new List<Customer>();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<FindAllByPhoneNumberListResultModel>> FindAllByPhoneNumberList([FromBody] FindAllByPhoneNumberListActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (actionModel.PhoneNumberList == null) throw new InvalidOperationException($"{nameof(actionModel.PhoneNumberList)} is empty");

            // Find
            var result = await _customerContext.CustomerRepository.FindAllByPhoneNumberListAsync(actionModel.PhoneNumberList);

            // Return
            return new FindAllByPhoneNumberListResultModel()
            {
                CustomerList = result
            };
        }


        // Class
        public class FindAllByPhoneNumberListActionModel
        {
            // Properties
            public List<string> PhoneNumberList { get; set; } = new List<string>();
        }

        public class FindAllByPhoneNumberListResultModel
        {
            // Properties
            public List<Customer> CustomerList { get; set; } = new List<Customer>();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<SearchAllByNameAndPhoneResultModel>> SearchAllBySearchString([FromBody] SearchAllByNameAndPhoneActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (string.IsNullOrEmpty(actionModel.AgentId) == true) throw new InvalidOperationException($"{nameof(actionModel.AgentId)} is empty");
            if (string.IsNullOrEmpty(actionModel.Value) == true) throw new InvalidOperationException($"{nameof(actionModel.Value)} is empty");
            
            // Fields
            var name = "";
            var phone = "";

            // Parsing Vaule string
            this.ParseInputString(actionModel.Value, out name, out phone);
            if (string.IsNullOrEmpty(name) == true && string.IsNullOrEmpty(phone) == true) throw new InvalidOperationException(nameof(actionModel));

            // Find
            var result = await _customerContext.CustomerRepository.SearchAllByNameAndPhoneAsync(actionModel.AgentId, name, phone, actionModel.OnlyOwner, actionModel.Limit);

            // Return
            return new SearchAllByNameAndPhoneResultModel()
            {
                CustomerList = result
            };
        }

        private void ParseInputString(string input, out string name, out string phone)
        {
            // Requirement
            if (string.IsNullOrEmpty(input) == true) throw new InvalidOperationException(nameof(input));

            // Parse
            phone = "";
            name = "";
            var splitStr = Regex.Split(input, "\\s+", RegexOptions.IgnoreCase);
            foreach (var str in splitStr)
            {
                int parse;
                if (int.TryParse(str, out parse))
                    phone = str;
                else
                    name = str;
            }
        }


        // Class
        public class SearchAllByNameAndPhoneActionModel
        {
            // Properties
            public string AgentId { get; set; }            // 業務 ID

            public string Value { get; set; }              // 包含姓名與電話的資料

            public bool OnlyOwner { get; set; }            // 只查擁有者的資料

            public int Limit { get; set; } = 100;          // SQL 搜尋長度限制
        }

        public class SearchAllByNameAndPhoneResultModel
        {
            // Properties
            public List<Customer> CustomerList { get; set; } = new List<Customer>();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<FindAllSameIdentityByCustomerIdListResultModel>> FindAllSameIdentityByCustomerIdList([FromBody] FindAllSameIdentityByCustomerIdListActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (actionModel.CustomerIdList == null) throw new InvalidOperationException($"{nameof(actionModel.CustomerIdList)} is empty");

            // Find
            var result = await _customerContext.CustomerRepository.FindAllSameIdentityByCustomerIdListAsync(actionModel.CustomerIdList);

            // Return
            return new FindAllSameIdentityByCustomerIdListResultModel()
            {
                CustomerList = result
            };
        }


        // Class
        public class FindAllSameIdentityByCustomerIdListActionModel
        {
            // Properties
            public List<string> CustomerIdList { get; set; } = new List<string>();
        }

        public class FindAllSameIdentityByCustomerIdListResultModel
        {
            // Properties
            public List<Customer> CustomerList { get; set; } = new List<Customer>();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<FindAllByIdentityNumberListResultModel>> FindAllByIdentityNumberList([FromBody] FindAllByIdentityNumberListActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (actionModel.IdentityNumberList == null) throw new InvalidOperationException($"{nameof(actionModel.IdentityNumberList)} is empty");

            // Find
            var result = await _customerContext.CustomerRepository.FindAllByIdentityNumberListAsync(actionModel.IdentityNumberList);

            // Return
            return new FindAllByIdentityNumberListResultModel()
            {
                CustomerList = result
            };
        }


        // Class
        public class FindAllByIdentityNumberListActionModel
        {
            // Properties
            public List<string> IdentityNumberList { get; set; } = new List<string>();
        }

        public class FindAllByIdentityNumberListResultModel
        {
            // Properties
            public List<Customer> CustomerList { get; set; } = new List<Customer>();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<FindAllByAgentIdResultModel>> FindAllByAgentId([FromBody] FindAllByAgentIdActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (string.IsNullOrEmpty(actionModel.AgentId) == true) throw new InvalidOperationException($"{nameof(actionModel.AgentId)} is empty");

            // Search
            var customerList = await _customerContext.CustomerRepository.FindAllByAgentIdAsync(actionModel.AgentId);

            // Return
            return new FindAllByAgentIdResultModel()
            {
                CustomerList = customerList
            };
        }


        // Class
        public class FindAllByAgentIdActionModel
        {
            // Properties
            public string AgentId { get; set; }
        }

        public class FindAllByAgentIdResultModel
        {
            // Properties
            public List<Customer> CustomerList { get; set; } = new List<Customer>();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<FindAllCustomerIdByPhoneResultModel>> FindAllCustomerIdByPhone([FromBody] FindAllCustomerIdByPhoneActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (string.IsNullOrEmpty(actionModel.PhoneNumber) == true) throw new InvalidOperationException($"{nameof(actionModel.PhoneNumber)} is empty");

            // Find
            var resultList = await _customerContext.CustomerRepository.FindAllCustomerIdByPhoneAsync(actionModel.PhoneNumber);

            // Return
            return new FindAllCustomerIdByPhoneResultModel()
            {
                CustomerIdList = resultList
            };
        }


        // Class
        public class FindAllCustomerIdByPhoneActionModel
        {
            // Properties
            public string PhoneNumber { get; set; }
        }

        public class FindAllCustomerIdByPhoneResultModel
        {
            // Properties
            public List<string> CustomerIdList { get; set; } = new List<string>();
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<FindAllRecordsFromMonthAgoResultModel>> FindAllRecordsFromMonthAgo([FromBody] FindAllRecordsFromMonthAgoActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (string.IsNullOrEmpty(actionModel.AgentId) == true) throw new InvalidOperationException($"{nameof(actionModel.AgentId)} is empty");

            // Find
            var resultList = await _customerContext.CustomerRepository.FindAllAfterUpdateDateTimeAsync(actionModel.AgentId, DateTime.Now.AddMonths(-actionModel.Month));

            // Return
            return new FindAllRecordsFromMonthAgoResultModel()
            {
                CustomerList = resultList
            };
        }


        // Class
        public class FindAllRecordsFromMonthAgoActionModel
        {
            // Properties
            public string AgentId { get; set; }

            public int Month { get; set; }
        }

        public class FindAllRecordsFromMonthAgoResultModel
        {
            // Properties
            public List<Customer> CustomerList { get; set; } = new List<Customer>();
        }
    }
    
    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<CopyCustomerResultModel>> CopyCustomer([FromBody] CopyCustomerActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (string.IsNullOrEmpty(actionModel.CustomerId) == true) throw new InvalidOperationException($"{nameof(actionModel.CustomerId)} is empty");

            // Find
            var resultList = await _customerContext.CopyCustomerService.CopyCustomerAsync(actionModel.CustomerId, actionModel.AgentId, actionModel.SinyiIdentityType);

            // Return
            return new CopyCustomerResultModel()
            {
                NewCustomerId = resultList
            };
        }


        // Class
        public class CopyCustomerActionModel
        {
            // Properties
            public string CustomerId { get; set; }

            public string AgentId { get; set; }

            public string SinyiIdentityType { get; set; }
        }

        public class CopyCustomerResultModel
        {
            // Properties
            public string NewCustomerId { get; set; }
        }
    }

    public partial class CustomerController : Controller
    {
        // Methods
        public async Task<ActionResult<CopyCommunityResultModel>> CopyCommunity([FromBody] CopyCommunityActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Find
            var result = await _customerContext.CopyCustomerService.CopyCommunityAsync(actionModel.UpdateAfter);

            // Return
            return new CopyCommunityResultModel()
            {
                Count = result
            };
        }


        // Class
        public class CopyCommunityActionModel
        {
            // Properties
            public DateTime UpdateAfter { get; set; }
        }

        public class CopyCommunityResultModel
        {
            // Properties
            public int Count { get; set; }
        }
    }
}
