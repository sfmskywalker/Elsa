﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Telnyx.Client.Models;
using Elsa.Activities.Telnyx.Client.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Exceptions;
using Elsa.Services;
using Elsa.Services.Models;
using Refit;

namespace Elsa.Activities.Telnyx.Activities
{
    [Action(
        Category = Constants.Category,
        Description = "Hang up the call.",
        Outcomes = new[] { OutcomeNames.Done },
        DisplayName = "Hangup Call"
    )]
    public class HangupCall : Activity
    {
        private readonly ITelnyxClient _telnyxClient;

        public HangupCall(ITelnyxClient telnyxClient)
        {
            _telnyxClient = telnyxClient;
        }

        [ActivityProperty(Label = "Call Control ID", Hint = "Unique identifier and token for controlling the call", Category = PropertyCategories.Advanced)]
        public string CallControlId { get; set; } = default!;
        
        [ActivityProperty(Label = "Client State", Hint = "Use this field to add state to every subsequent webhook. It must be a valid Base-64 encoded string.", Category = PropertyCategories.Advanced)]
        public string? ClientState { get; set; }

        [ActivityProperty(Label = "Command ID", Hint = "Use this field to avoid duplicate commands. Telnyx will ignore commands with the same Command ID.", Category = PropertyCategories.Advanced)]
        public string? CommandId { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            await HangupCallAsync(context);
            return Done();
        }

        private async ValueTask HangupCallAsync(ActivityExecutionContext context)
        {
            var callControlId = CallControlId is not null and not "" ? CallControlId : context.CorrelationId ?? throw new InvalidOperationException("Cannot answer call without a call control ID");
            var request = new HangupCallRequest(ClientState, CommandId);
            
            try
            {
                await _telnyxClient.Calls.HangupCallAsync(callControlId, request, context.CancellationToken);
            }
            catch (ApiException e)
            {
                throw new WorkflowException(e.Content ?? e.Message, e);
            }
        }
    }
}