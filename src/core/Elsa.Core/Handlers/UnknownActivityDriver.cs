using Elsa.Activities;
using Elsa.Models;
using Elsa.Results;

namespace Elsa.Handlers
{
    public class UnknownActivityDriver : ActivityDriver<UnknownActivity>
    {
        protected override ActivityExecutionResult OnExecute(UnknownActivity activity, WorkflowExecutionContext workflowContext)
        {
            return Fault($"Unknown activity: {activity.TypeName}, ID: {activity.Id}");
        }
    }
}