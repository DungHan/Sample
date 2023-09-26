using MDP.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.SuperAgent.WebPlatform
{
    public static class Host
    {
        // Methods
        public static IHost CreateHost(string[] args)
        {
            #region Contracts

            if (args == null) throw new ArgumentException(nameof(args));

            #endregion

            // Host
            // return MDP.AspNetCore.Host.CreateHostBuilder<Startup>(args);
            return MDP.AspNetCore.Host.Create(args);
        }
    }
}
