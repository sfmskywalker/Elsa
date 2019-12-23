using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.MassTransit.Options;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Scripting;
using Elsa.Services.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Elsa.Activities.MassTransit.Activities
{
    [ActivityDefinition(
        Category = "MassTransit",
        DisplayName = "Schedule MassTransit Message",
        Description = "Schedule a message via MassTransit."
    )]
    public class ScheduleSendMassTransitMessage : MassTransitBusActivity
    {
        private readonly MessageScheduleOptions options;

        public ScheduleSendMassTransitMessage(IBus bus, ConsumeContext consumeContext, IOptions<MessageScheduleOptions> options)
        : base(bus, consumeContext)
        {
            this.options = options.Value;
        }

        [ActivityProperty(Hint = "The assembly-qualified type name of the message to send.")]
        public Type MessageType
        {
            get
            {
                var typeName = GetState<string>();
                return string.IsNullOrWhiteSpace(typeName) ? null : System.Type.GetType(typeName);
            }
            set => SetState(value.AssemblyQualifiedName);
        }

        [ActivityProperty(Hint = "An expression that evaluates to the message to be delivered.")]
        public IWorkflowExpression Message
        {
            get => GetState<IWorkflowExpression>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "The address of a specific endpoint to deliver the message to.")]
        public Uri EndpointAddress
        {
            get
            {
                var endpointAddress = GetState<string>();
                return string.IsNullOrEmpty(endpointAddress) ? null : new Uri(endpointAddress);
            }
            set => SetState(value.ToString());
        }

        [ActivityProperty(Hint = "An expression that evaluates to the date and time to deliver the message.")]
        public IWorkflowExpression<DateTime> ScheduledTime
        {
            get => GetState<IWorkflowExpression<DateTime>>();
            set => SetState(value);
        }

        protected override bool OnCanExecute(WorkflowExecutionContext context)
        {
            return MessageType != null && options.SchedulerAddress != null;
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var message = await context.EvaluateAsync(Message, MessageType, cancellationToken);
            var scheduledTime = await context.EvaluateAsync(ScheduledTime, cancellationToken);
            var endpoint = await SendEndpointProvider.GetSendEndpoint(options.SchedulerAddress);
            var scheduledMessage = await endpoint.ScheduleSend(EndpointAddress, scheduledTime, message, cancellationToken);

            return Done(scheduledMessage.TokenId);
        }
    }
}