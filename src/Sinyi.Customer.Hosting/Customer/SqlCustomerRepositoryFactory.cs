using Autofac;
using Sinyi.Data.MsSqlClient;
using MDP.Hosting;
using Microsoft.Extensions.Hosting;
using Sinyi.Customers.Accesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Customers.Hosting
{
    public class SqlCustomerRepositoryFactory : ServiceFactory<CustomerRepository, SqlCustomerRepository, SqlCustomerRepositoryFactory.Setting>
    {
        // Methods
        protected override SqlCustomerRepository CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new SqlCustomerRepository
            (
                componentContext.Resolve<MsSqlClientFactory>(),
                componentContext.Resolve<IHostEnvironment>().EnvironmentName
            );
        }


        // Class
        public class Setting
        {

        }
    }
}
