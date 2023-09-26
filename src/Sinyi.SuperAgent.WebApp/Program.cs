using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sinyi.Data.MsSqlClient.Hosting;
using System.Threading;

namespace Sinyi.SuperAgent.WebApp
{
    public class Program
    {
        // Methods
        public static void Main(string[] args)
        {
            // Set
            ThreadPool.SetMinThreads(200, 20);

            // Monitor
            MonitorThreadUsage();

            // Host
            Sinyi.SuperAgent.WebPlatform.Host.CreateHost(args).Run();
        }

        private static void MonitorThreadUsage()
        {
            // Fields
            var cts = new CancellationTokenSource();

            // Write
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
            Console.WriteLine($"ThreadPool Min: {minWorkerThreads} {minCompletionPortThreads}");
            Console.WriteLine($"ThreadPool Max: {maxWorkerThreads} {maxCompletionPortThreads}");

            // Watch
            Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine($"MonitorThreadUsage: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} | Usage {ThreadPool.ThreadCount,2}");
                    await Task.Delay(10000);
                }
            }, cts.Token);
        }
    }
}
