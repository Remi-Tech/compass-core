using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Compass.KafkaListener
{
    class Program
    {
        static void Main(string[] args)
        {
            string brokerList = args[0];
            var topics = args.Skip(1).ToList();


            using (var consumer = new Consumer<Null, string>(GetConfig(brokerList), null, new StringDeserializer(Encoding.UTF8)))
            {
                RegisterDelegates(consumer);

                consumer.Subscribe(topics);

                Console.WriteLine($"Subscribed to: [{string.Join(", ", consumer.Subscription)}]");

                while (true)
                {
                    consumer.Poll(100);
                }
            }
        }

        private static void RegisterDelegates(Consumer<Null, string> consumer)
        {
            consumer.OnMessage +=
                                (_, msg) => Console.WriteLine(
                                    $"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

            consumer.OnPartitionEOF += (_, end)
                => Console.WriteLine($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

            consumer.OnError += (_, error)
                => Console.WriteLine($"Error: {error}");

            consumer.OnConsumeError += (_, msg)
                => Console.WriteLine($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");

            consumer.OnOffsetsCommitted += (_, commit) =>
            {
                Console.WriteLine($"[{string.Join(", ", commit.Offsets)}]");

                if (commit.Error)
                {
                    Console.WriteLine($"Failed to commit offsets: {commit.Error}");
                }
                Console.WriteLine($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
            };

            consumer.OnPartitionsAssigned += (_, partitions) =>
            {
                Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                consumer.Assign(partitions);
            };

            consumer.OnPartitionsRevoked += (_, partitions) =>
            {
                Console.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]");
                consumer.Unassign();
            };

            consumer.OnStatistics += (_, json)
                => Console.WriteLine($"Statistics: {json}");
        }

        private static Dictionary<string, object> GetConfig(string brokerList)
        {
            return new Dictionary<string, object>
            {
                {"group.id", Guid.NewGuid().ToString()},
                {"enable.auto.commit", true},
                {"auto.commit.interval.ms", 5000},
                {"statistics.interval.ms", 60000},
                {"bootstrap.servers", brokerList},
                {
                    "default.topic.config", new Dictionary<string, object>()
                    {
                        {"auto.offset.reset", "smallest"}
                    }
                }
            };
        }
    }
}
