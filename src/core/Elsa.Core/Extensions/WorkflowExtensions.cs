using System.Collections.Generic;
using System.Linq;
using Elsa.Activities.Containers;
using Elsa.Activities.Flowcharts;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Extensions
{
    public static class WorkflowExtensions
    {
        public static IEnumerable<IActivity> GetStartActivities(this Flowchart workflow)
        {
            var targetActivityIds = workflow.Connections.Select(x => x.Target.Activity.Id).Distinct().ToLookup(x => x);
            
            var query =
                from activity in workflow.Activities
                where !targetActivityIds.Contains(activity.Id)
                select activity;
            
            return query;
        }

        public static IActivity GetActivity(this Flowchart workflow, string id) =>
            workflow.Activities.FirstOrDefault(x => x.Id == id);

        public static IEnumerable<Connection> GetInboundConnections(this Flowchart workflow, string activityId)
        {
            return workflow.Connections.Where(x => x.Target.Activity.Id == activityId).ToList();
        }

        public static IEnumerable<Connection> GetOutboundConnections(this Flowchart workflow, string activityId)
        {
            return workflow.Connections.Where(x => x.Source.Activity.Id == activityId).ToList();
        }

        /// <summary>
        /// Returns the full path of incoming activities.
        /// </summary>
        public static IEnumerable<string> GetInboundActivityPath(this Flowchart workflow, string activityId)
        {
            return workflow.GetInboundActivityPathInternal(activityId, activityId).Distinct().ToList();
        }

        private static IEnumerable<string> GetInboundActivityPathInternal(this Flowchart workflowInstance, string activityId, string startingPointActivityId)
        {
            foreach (var connection in workflowInstance.GetInboundConnections(activityId))
            {
                // Circuit breaker: Detect workflows that implement repeating flows to prevent an infinite loop here.
                if (connection.Source.Activity.Id == startingPointActivityId)
                    yield break;

                yield return connection.Source.Activity.Id;

                foreach (var parentActivityId in workflowInstance
                    .GetInboundActivityPathInternal(connection.Source.Activity.Id, startingPointActivityId)
                    .Distinct())
                    yield return parentActivityId;
            }
        }
    }
}