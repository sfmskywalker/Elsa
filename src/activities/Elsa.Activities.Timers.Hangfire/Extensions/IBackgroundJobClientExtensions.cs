using System;
using System.Linq;

using Elsa.Activities.Timers.Hangfire.Jobs;
using Elsa.Activities.Timers.Hangfire.Models;

using Hangfire;

namespace Elsa.Activities.Timers
{
    public static class IBackgroundJobClientExtensions
    {
        public static void ScheduleWorkflow(this IBackgroundJobClient backgroundJobClient, RunHangfireWorkflowJobModel data)
        {
            if (data.DateTimeOffset.HasValue == false)
            {
                throw new ArgumentNullException(nameof(data.DateTimeOffset));
            }

            UnscheduleJobWhenAlreadyExists(data);
            backgroundJobClient.Schedule<RunHangfireWorkflowJob>(job => job.ExecuteAsync(data), data.DateTimeOffset.Value);
        }

        public static void UnscheduleJobWhenAlreadyExists(RunHangfireWorkflowJobModel data)
        {
            var identity = data.GetIdentity();
            if (data.RecurringJob)
            {
                var monitor = JobStorage.Current.GetMonitoringApi();
                var workflowJobType = typeof(RunHangfireWorkflowJob);

                var jobs = monitor.ScheduledJobs(0, int.MaxValue)
                   .Where(x => x.Value.Job.Type == workflowJobType && ((RunHangfireWorkflowJobModel)x.Value.Job.Args[0]).GetIdentity() == identity);

                foreach(var job in jobs)
                {
                    BackgroundJob.Delete(job.Key);
                }
            }
            else
            {
                RecurringJob.RemoveIfExists(identity);
            }
        }
    }
}
