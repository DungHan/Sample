using Sinyi.Data.MsSqlClient;
using Sinyi.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Sinyi.Customers.Accesses
{
    // SA 客戶表，只寫特殊欄位，基本欄位靠 IntraCustomerRepository 寫到內網表
    public partial class SqlCustomerRepository : CustomerRepository
    {
        // Fields
        private readonly MsSqlClientFactory _sqlClientFactory = null;

        private readonly string _environmentName = null;


        // Constructors
        public SqlCustomerRepository(MsSqlClientFactory sqlClientFactory, string environmentName)
        {
            #region Contracts

            if (sqlClientFactory == null) throw new ArgumentException(nameof(sqlClientFactory));

            #endregion

            // Default
            _sqlClientFactory = sqlClientFactory;
            _environmentName = environmentName;
        }


        // Methods
        public async Task<Customer> FindByCustomerIdAsync(string customerId, string agentId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(customerId) == true) throw new ArgumentException(nameof(customerId));

            #endregion

            // FindAll
            using (var command = _sqlClientFactory.CreateCommand("DefaultConnection"))
            {
                // CommandParameters
                command.AddParameter(nameof(customerId), customerId, SqlDbType.VarChar);
                command.AddParameter("@LIMIT", 100, SqlDbType.Int);
                if (string.IsNullOrEmpty(agentId) != true) command.AddParameter(nameof(agentId), agentId, SqlDbType.VarChar);

                // CommandText
                command.CommandText = CreateCommand();

                // Execute
                var result = await command.ExecuteParseAsync<Customer>();

                // Return
                return result;
            }


            // SubFunction
            string CreateCommand()
            {
                // Field
                var sb = new StringBuilder();

                // Create
                sb.AppendLine(this.SelectSuperAgentSqlString());
                sb.AppendLine("WHERE 客戶.CUSTOMER_ID = @customerId");
                if (string.IsNullOrEmpty(agentId) != true) sb.AppendLine("And 客戶.AGENT_ID = @agentId");

                // Return
                return sb.ToString();
            }
        }


        // Sql string
        private string SelectSuperAgentSqlString()
        {
            // Return
            return @"SELECT TOP (@LIMIT) [CUSTOMER_ID] AS CustomerId
                                        ,[AGENT_ID] AS AgentId
                                        ,[CITY] AS City
                                        ,[DISTRICT] AS District
                                        ,[COMMUNITY] AS CommunityName
                                        ,[SOURCE_TYPE_ID] AS SourceTypeId
                                        ,[UPDATE_DATE] AS UpdateDate
                     FROM [SuperAP].[dbo].[SAP_CUSTOMERS_CUSTOMER_I] AS 客戶 WITH (NOLOCK)" + (char)13;
        }
    }
}
