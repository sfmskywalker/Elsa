﻿using System;
using System.Threading.Tasks;
using Elsa.Activities.Console.Extensions;
using Elsa.Core.Extensions;
using Elsa.Services;
using Elsa.Services.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Sample04.Activities;

namespace Sample04
{
    /// <summary>
    /// A strongly-typed workflows program demonstrating scripting, and branching.
    /// </summary>
    class Program
    {
        static async Task Main()
        {
            // Setup a service collection.
            var services = new ServiceCollection()
                .AddWorkflows()
                .AddConsoleActivities()
                .AddActivity<Sum>()
                .AddActivity<Subtract>()
                .AddActivity<Multiply>()
                .AddActivity<Divide>()
                .BuildServiceProvider();

            // Create a workflow.
            var workflowFactory = services.GetRequiredService<IWorkflowFactory>();
            var workflow = workflowFactory.CreateWorkflow<CalculatorWorkflow>();

            // Invoke the workflow.
            var invoker = services.GetService<IWorkflowInvoker>();
            await invoker.InvokeAsync(workflow);

            Console.ReadLine();
        }
    }
}