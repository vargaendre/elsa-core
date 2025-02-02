﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Exceptions;

namespace Elsa.Services.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceBusFactory _serviceBusFactory;
        private readonly ILogger<EventPublisher> _logger;
        private readonly SemaphoreSlim _semaphore = new(1);

        public EventPublisher(IServiceBusFactory serviceBusFactory, ILogger<EventPublisher> logger)
        {
            _serviceBusFactory = serviceBusFactory;
            _logger = logger;
        }

        public async Task PublishAsync(object message, IDictionary<string, string>? headers = default, CancellationToken cancellationToken = default)
        {
            // Attempt to prevent: Could not 'GetOrAdd' item with key 'new-azure-service-bus-transport' error.
            await _semaphore.WaitAsync(cancellationToken);
            
            try
            {
                var bus = await _serviceBusFactory.GetServiceBusAsync(message.GetType(), cancellationToken: cancellationToken);
                await bus.Publish(message, headers);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}