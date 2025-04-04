using System;
using BookMe.Application.Configurations;
using BookMe.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BookMe.Infrastructure.Events;

public class KafkaProducer : IEventPublisher
{
    private readonly EventConfig _eventConfig;
    private readonly ILogger<KafkaProducer> _logger;
    public KafkaProducer(IOptionsSnapshot<AppSettings> appSettings, ILogger<KafkaProducer> logger)
    {
        _eventConfig = appSettings.Value.EventConfig;
        _logger = logger;
    }
    public async Task PublishAsync<TEvent>(TEvent @event)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _eventConfig.Server
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        try
        {
            var deliveryResult = await producer.ProduceAsync(_eventConfig.Topic, new Message<string, string>
            {
                Key = @event.GetType().Name,
                Value = JsonConvert.SerializeObject(@event)
            });

            _logger.LogInformation("Message sent to {@deliveryResult}", deliveryResult.TopicPartitionOffset);
        }
        catch (ProduceException<string, string> e)
        {
            _logger.LogError("Error sending message: {@error}", e.Error.Reason);
        }
    }
}
