using System;

namespace Compass.Shared
{
    public class CompassEnvironment : ICompassEnvironment
    {
        public Uri GetCouchbaseUri()
        {
            return new Uri(Environment.GetEnvironmentVariable("CompassCouchbaseUri"));
        }

        public string GetCouchbaseBucketName()
        {
            return Environment.GetEnvironmentVariable("CompassCouchbaseBucketName");
        }

        public uint GetSubscriptionTtl()
        {
            // If we can't parse the Subscrption Time To Live from the environment,
            // use a default of 30 seconds.
            return uint.TryParse(Environment.GetEnvironmentVariable("SubscriptionTtl"), out uint subscriptionTtl)
                ? subscriptionTtl
                : 30000;
        }

        public string GetKafkaTopic()
        {
            return Environment.GetEnvironmentVariable("KafkaTopic") ?? "Compass";
        }

        public string GetKafkaBrokerList()
        {
            return Environment.GetEnvironmentVariable("KafkaBrokerList");
        }

        public string GetCouchbaseUserName()
        {
            return Environment.GetEnvironmentVariable("CompassCouchbaseUserName");
        }

        public string GetCouchbasePassword()
        {
            return Environment.GetEnvironmentVariable("CompassCouchbasePassword");
        }

        public int GetCouchbaseQueryPort()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("CompassCouchbaseQueryPort"), out int port)
                ? port
                : 8093;
        }

        public int GetRetryAttempts()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("RetryAttempts"), out int attempts)
                ? attempts
                : 3;
        }

        public int GetRetrySleepDuration()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("RetrySleepDuration"), out int duration)
                ? duration
                : 8000;
        }

        public int GetRequestTimeout()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("RequestTimeout"), out int timeout)
                ? timeout
                : 3000;
        }

        public int GetExceptionsBeforeBreakingCircuit()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("ExceptionsBeforeBreakingCircuit"), out int exceptions)
                ? exceptions
                : 10;
        }

        public int GetCircuitBreakerDuration()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("CircuitBreakerDuration"), out int duration)
                ? duration
                : 8000;
        }

        public int GetKafkaProducerTimeout()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("KafkaProducerTimeout"), out int timeout)
                ? timeout
                : 100;
        }

        public static string AppInsightsKey => System.Environment.GetEnvironmentVariable("AppInsightsKey");

    }
}
