﻿using System;
using System.Collections.Generic;
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
        Description = "Transfer a call to a new destination",
        Outcomes = new[] { OutcomeNames.Done },
        DisplayName = "Transfer Call"
    )]
    public class TransferCall : Activity
    {
        private readonly ITelnyxClient _telnyxClient;

        public TransferCall(ITelnyxClient telnyxClient)
        {
            _telnyxClient = telnyxClient;
        }

        [ActivityProperty(Label = "Call Control ID", Hint = "Unique identifier and token for controlling the call.", Category = PropertyCategories.Advanced)]
        public string CallControlId { get; set; } = default!;

        [ActivityProperty(Label = "To", Hint = "The DID or SIP URI to dial out and bridge to the given call.")]
        public string To { get; set; }

        [ActivityProperty(Label = "From",
            Hint = "The 'from' number to be used as the caller id presented to the destination ('To' number). The number should be in +E164 format. This attribute will default to the 'From' number of the original call if omitted.")]
        public string? From { get; set; }

        [ActivityProperty(Label = "From Display Name",
            Hint =
                "The string to be used as the caller id name (SIP From Display Name) presented to the destination ('To' number). The string should have a maximum of 128 characters, containing only letters, numbers, spaces, and -_~!.+ special characters. If omitted, the display name will be the same as the number in the 'From' field.")]
        public string? FromDisplayName { get; set; }

        [ActivityProperty(Label = "Answering Machine Detection", Hint = "Enables Answering Machine Detection.", UIHint = ActivityPropertyUIHints.Dropdown,
            Options = new[] { "disabled", "detect", "detect_beep", "detect_words", "greeting_end" })]
        public string? AnsweringMachineDetection { get; set; }

        [ActivityProperty(Label = "Answering Machine Detection Configuration", Hint = "Optional configuration parameters to modify answering machine detection performance.", Category = PropertyCategories.Advanced)]
        public AnsweringMachineConfig? AnsweringMachineDetectionConfig { get; set; }

        [ActivityProperty(Label = "Command ID", Hint = "Use this field to avoid duplicate commands. Telnyx will ignore commands with the same Command ID.", Category = PropertyCategories.Advanced)]
        public string? CommandId { get; set; }

        [ActivityProperty(Label = "Audio URL", Hint = "Audio URL to be played back when the transfer destination answers before bridging the call. The URL can point to either a WAV or MP3 file.")]
        public Uri? AudioUrl { get; set; }

        [ActivityProperty(Label = "Client State", Hint = "Use this field to add state to every subsequent webhook. It must be a valid Base-64 encoded string.", Category = PropertyCategories.Advanced)]
        public string? ClientState { get; set; }

        [ActivityProperty(Label = "Target Leg Client State", Hint = "Use this field to add state to every subsequent webhook for the new leg. It must be a valid Base-64 encoded string.", Category = PropertyCategories.Advanced)]
        public string? TargetLegClientState { get; set; }

        [ActivityProperty(Label = "Custom Headers", Hint = "Custom headers to be added to the SIP INVITE.", Category = PropertyCategories.Advanced)]
        public IList<Header>? CustomHeaders { get; set; }

        [ActivityProperty(Label = "SIP Authentication Username", Hint = "SIP Authentication username used for SIP challenges.", Category = "SIP Authentication")]
        public string? SipAuthUsername { get; set; }

        [ActivityProperty(Label = "SIP Authentication Password", Hint = "SIP Authentication password used for SIP challenges.", Category = "SIP Authentication")]
        public string? SipAuthPassword { get; set; }

        [ActivityProperty(Label = "Time Limit", Hint = "Sets the maximum duration of a Call Control Leg in seconds.", Category = PropertyCategories.Advanced)]
        public int? TimeLimitSecs { get; set; }

        [ActivityProperty(Label = "Timeout", Hint = "The number of seconds that Telnyx will wait for the call to be answered by the destination to which it is being transferred.", Category = PropertyCategories.Advanced)]
        public int? TimeoutSecs { get; set; }

        [ActivityProperty(Label = "Webhook URL", Hint = "Use this field to override the URL for which Telnyx will send subsequent webhooks to for this call.", Category = PropertyCategories.Advanced)]
        public string? WebhookUrl { get; set; }

        [ActivityProperty(Label = "Webhook URL Method", Hint = "HTTP request type used for Webhook URL", UIHint = ActivityPropertyUIHints.Dropdown, Options = new[] { "GET", "POST" }, Category = PropertyCategories.Advanced)]
        public string? WebhookUrlMethod { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            await TransferCallAsync(context);
            return Done();
        }

        private async ValueTask TransferCallAsync(ActivityExecutionContext context)
        {
            var request = new TransferCallRequest(
                To,
                From,
                FromDisplayName,
                AudioUrl,
                AnsweringMachineDetection,
                AnsweringMachineDetectionConfig,
                ClientState,
                TargetLegClientState,
                CommandId,
                CustomHeaders,
                SipAuthUsername,
                SipAuthPassword,
                TimeLimitSecs,
                TimeoutSecs,
                WebhookUrl,
                WebhookUrlMethod
            );

            var callControlId = CallControlId is not null and not "" ? CallControlId : context.CorrelationId ?? throw new InvalidOperationException("Cannot answer call without a call control ID");
            
            try
            {
                await _telnyxClient.Calls.TransferCallAsync(callControlId, request, context.CancellationToken);
            }
            catch (ApiException e)
            {
                throw new WorkflowException(e.Content ?? e.Message, e);
            }
        }
    }
}