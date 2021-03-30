﻿using System.Threading;
using System.Threading.Tasks;
using Elsa.Dispatch;
using Elsa.Models;
using Elsa.Server.Orleans.Grains.Contracts;
using Elsa.Services;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Elsa.Server.Orleans.Grains
{
    public class WorkflowDefinitionGrain : Grain, IWorkflowDefinitionGrain
    {
        private readonly IWorkflowRegistry _workflowRegistry;
        private readonly IWorkflowRunner _workflowRunner;
        private readonly ILogger<WorkflowDefinitionGrain> _logger;

        public WorkflowDefinitionGrain(IWorkflowRegistry workflowRegistry, IWorkflowRunner workflowRunner, ILogger<WorkflowDefinitionGrain> logger)
        {
            _workflowRegistry = workflowRegistry;
            _workflowRunner = workflowRunner;
            _logger = logger;
        }
        
        public async Task ExecuteWorkflowAsync(ExecuteWorkflowDefinitionRequest request, CancellationToken cancellationToken = default)
        {
            var workflowBlueprint = await _workflowRegistry.GetWorkflowAsync(request.WorkflowDefinitionId, request.TenantId, VersionOptions.Published, cancellationToken);

            if (workflowBlueprint == null)
            {
                _logger.LogWarning("No published workflow definition {WorkflowDefinitionId} found", request.WorkflowDefinitionId);
                return;
            }

            await _workflowRunner.RunWorkflowAsync(workflowBlueprint, request.ActivityId, request.Input, request.CorrelationId, request.ContextId, cancellationToken);
        }
    }
}