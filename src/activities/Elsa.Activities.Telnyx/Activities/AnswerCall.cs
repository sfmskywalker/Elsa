﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Telnyx.Client.Models;
using Elsa.Activities.Telnyx.Client.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Activities.Telnyx.Activities
{
    [Action(
        Category = Constants.Category,
        Description = "Answer an incoming call. You must issue this command before executing subsequent commands on an incoming call",
        Outcomes = new[] { OutcomeNames.Done },
        DisplayName = "Answer Call"
    )]
    public class AnswerCall : Activity
    {
        private readonly ITelnyxClient _telnyxClient;

        public AnswerCall(ITelnyxClient telnyxClient)
        {
            _telnyxClient = telnyxClient;
        }

        [ActivityProperty(Label = "Call Control ID", Hint = "Unique identifier and token for controlling the call", Category = PropertyCategories.Advanced)]
        public string CallControlId { get; set; } = default!;

        [ActivityProperty(Label = "Billing Group ID", Hint = "Use this field to set the Billing Group ID for the call. Must be a valid and existing Billing Group ID.", Category = PropertyCategories.Advanced)]
        public string? BillingGroupId { get; set; }

        [ActivityProperty(Label = "Client State", Hint = "Use this field to add state to every subsequent webhook. It must be a valid Base-64 encoded string.", Category = PropertyCategories.Advanced)]
        public string? ClientState { get; set; }

        [ActivityProperty(Label = "Command ID", Hint = "Use this field to avoid duplicate commands. Telnyx will ignore commands with the same Command ID.", Category = PropertyCategories.Advanced)]
        public string? CommandId { get; set; }

        [ActivityProperty(Label = "Webhook URL", Hint = "Use this field to override the URL for which Telnyx will send subsequent webhooks to for this call.", Category = PropertyCategories.Advanced)]
        public string? WebhookUrl { get; set; }

        [ActivityProperty(Label = "Webhook URL Method", Hint = "HTTP request type used for Webhook URL", UIHint = ActivityPropertyUIHints.Dropdown, Options = new[] { "GET", "POST" }, Category = PropertyCategories.Advanced)]
        public string? WebhookUrlMethod { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            await AnswerCallAsync(context);
            return Done();
        }

        private async ValueTask AnswerCallAsync(ActivityExecutionContext context)
        {
            var callControlId = CallControlId is not null and not "" ? CallControlId : context.CorrelationId ?? throw new InvalidOperationException("Cannot answer call without a call control ID");
            var request = new AnswerCallRequest(BillingGroupId, ClientState, CommandId, WebhookUrl, WebhookUrlMethod);
            await _telnyxClient.Calls.AnswerCallAsync(callControlId, request, context.CancellationToken);
        }
    }
}