using System;
using System.Runtime.CompilerServices;
using Elsa.Services.Models;

namespace Elsa.Builders
{
    public interface IOutcomeBuilder : IBuilder
    {
        ICompositeActivityBuilder WorkflowBuilder { get; }
        IActivityBuilder Source { get; }
        string? Outcome { get; }
        IConnectionBuilder Then(string activityName);
        IConnectionBuilder Then(IActivityBuilder targetActivity, Action<IActivityBuilder>? branch = default);
        IWorkflowBlueprint Build();
    }
}