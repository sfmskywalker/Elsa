﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Elsa.Models;

namespace Elsa.Persistence.Specifications.WorkflowTriggers
{
    public class WorkflowInstanceIdsSpecification : Specification<WorkflowTrigger>
    {
        public WorkflowInstanceIdsSpecification(IEnumerable<string> workflowInstanceIds)
        {
            WorkflowInstanceIds = workflowInstanceIds;
        }

        public IEnumerable<string> WorkflowInstanceIds { get; }

        public override Expression<Func<WorkflowTrigger, bool>> ToExpression() => trigger => WorkflowInstanceIds.Contains(trigger.WorkflowInstanceId);
    }
}