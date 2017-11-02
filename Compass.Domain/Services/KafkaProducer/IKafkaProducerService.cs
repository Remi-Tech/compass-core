using Compass.Domain.Models;

namespace Compass.Domain.Services.KafkaProducer
{
    public interface IKafkaProducerService
    {
        void Produce(CompassEvent compassEvent);
    }
}
