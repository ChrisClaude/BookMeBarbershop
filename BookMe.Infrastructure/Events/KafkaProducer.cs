using System;
using BookMe.Application.Configurations;
using BookMe.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace BookMe.Infrastructure.Events;

public class KafkaProducer : IEventPublisher
{
    private readonly EventConfig _eventConfig;
    public KafkaProducer(IOptionsSnapshot<AppSettings> appSettings)
    {
        _eventConfig = appSettings.Value.EventConfig;
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

            Log.Information("Message sent to {@deliveryResult}", deliveryResult.TopicPartitionOffset);
        }
        catch (ProduceException<string, string> e)
        {
            Log.Error("Error sending message: {@error}", e.Error.Reason);
        }
    }
}
