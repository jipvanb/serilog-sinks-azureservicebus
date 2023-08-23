using Azure.Messaging.ServiceBus;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Text;
using System.Threading;

namespace Serilog.Sinks.AzureServiceBus.Alternate
{
    /// <summary>
    /// Writes log events as records to an Azure ServiceBus queue.
    /// </summary>
    public class AzureServiceBusSink : ILogEventSink
    {
        readonly int _waitTimeoutInMilliseconds = Timeout.Infinite;
        readonly ServiceBusSender _serviceBusSender;
        readonly IFormatProvider _formatProvider;

        public AzureServiceBusSink(
            ServiceBusSender serviceBusSender,
            IFormatProvider formatProvider)
        {
            _serviceBusSender = serviceBusSender;
            _formatProvider = formatProvider;
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {

            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(logEvent.RenderMessage(_formatProvider)));
            message.ApplicationProperties.Add("MessageType", "http://schemas.ores.be/eiam/messages/event/1.0#MessageReceivedEvent");

            _serviceBusSender.SendMessageAsync(message).Wait(_waitTimeoutInMilliseconds);
        }    
    }
}