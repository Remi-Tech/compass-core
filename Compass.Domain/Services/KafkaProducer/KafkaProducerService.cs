using System.Collections.Generic;
using System.Text;
using Compass.Domain.Models;
using Compass.Shared;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;

namespace Compass.Domain.Services.KafkaProducer
{
    /// <summary>
    /// This class is registered in the DI framework as a singleton 
    /// across the application lifecycle (not per request). The reason
    /// for this is to absorb the overhead of establishing the
    /// connection to Kafka once. With this approach, the time to
    /// produce events to Kafka is single-digit millisecond.
    /// </summary>
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly Producer<Null, string> _producer;

        public KafkaProducerService(
            ICompassEnvironment compassEnvironment
        )
        {
            _compassEnvironment = compassEnvironment;
            _producer = new Producer<Null, string>(GetProducerConfig(), null, new StringSerializer(Encoding.UTF8));
        }

        public void Produce(CompassEvent compassEvent)
        {
            _producer.ProduceAsync(_compassEnvironment.GetKafkaTopic(), null, JsonConvert.SerializeObject(compassEvent));
            _producer.Flush(_compassEnvironment.GetKafkaProducerTimeout());
        }

        private Dictionary<string, object> GetProducerConfig()
        {
            return new Dictionary<string, object> { { "bootstrap.servers", _compassEnvironment.GetKafkaBrokerList() } };
        }
    }
}
