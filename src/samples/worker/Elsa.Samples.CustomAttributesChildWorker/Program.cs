using Elsa.Samples.CustomAttributesChildWorker.Messages;
using Elsa.Samples.CustomAttributesChildWorker.Workflows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Elsa.Activities.Timers;

namespace Elsa.Samples.CustomAttributesChildWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (_, services) =>
                    {
                        services
                            .AddElsa(options => options
                                .AddTimerActivities(o => o.UseQuartzProvider())
                                .AddConsoleActivities()
                                .AddRebusActivities<OrderReceived>()
                                .AddWorkflow<GenerateOrdersWorkflow>()
                                .AddWorkflow<OrderReceivedWorkflow>()
                                .AddWorkflow<Customer1Workflow>()
                                .AddWorkflow<Customer2Workflow>())
                            ;
                    });
    }
}