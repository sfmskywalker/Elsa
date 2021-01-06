using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Timers.Services;
using Elsa.ActivityResults;
using Elsa.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace Elsa.Activities.Timers.ActivityResults
{
    public class ScheduleWorkflowResult : ActivityExecutionResult
    {
        public ScheduleWorkflowResult(Instant executeAt)
        {
            ExecuteAt = executeAt;
        }

        public Instant ExecuteAt { get; }

        protected override void Execute(ActivityExecutionContext activityExecutionContext)
        {
            var workflowInstanceId = activityExecutionContext.WorkflowExecutionContext.WorkflowInstance.Id;
            var activityId = activityExecutionContext.ActivityBlueprint.Id;
            var workflowBlueprint = activityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint;
            var executeAt = ExecuteAt;
            
            async ValueTask ScheduleWorkflowAsync(WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken)
            {
                var scheduler = workflowExecutionContext.ServiceScope.ServiceProvider.GetRequiredService<IWorkflowScheduler>(); 
                await scheduler.ScheduleWorkflowAsync(workflowBlueprint, workflowInstanceId, activityId, executeAt, cancellationToken);
            }

            activityExecutionContext.WorkflowExecutionContext.RegisterTask(nameof(ScheduleWorkflowResult), ScheduleWorkflowAsync);
        }
    }
}