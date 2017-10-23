using Compass.Domain.Models;

namespace Compass.Domain.Services.KafkaStream
{
    public interface IKafkaStreamService
    {
        void StreamToKafka(CompassEvent compassEvent);
    }
}
