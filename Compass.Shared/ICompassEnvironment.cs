using System;

namespace Compass.Shared
{
    /// <summary>
    /// A service for retrieving environment variables.
    /// </summary>
    public interface ICompassEnvironment
    {
        /// <summary>
        /// The URI to connect to a Couchbase instance.
        /// </summary>
        /// <returns></returns>
        Uri GetCouchbaseUri();

        /// <summary>
        /// The Coucbase bucket name. Defaults to Compass.
        /// </summary>
        /// <returns></returns>
        string GetCouchbaseBucketName();

        uint GetSubscriptionTtl();
        string GetKafkaTopic();
        string GetKafkaBrokerList();
        string GetCouchbaseUserName();
        string GetCouchbasePassword();
        int GetCouchbaseQueryPort();

        /// <summary>
        /// Get the number of times to retry a failed request
        /// before storing the event. Defaults to 3.
        /// </summary>
        /// <returns></returns>
        int GetRetryAttempts();

        /// <summary>
        /// Get the time, in milliseconds, to sleep between
        /// each retry attempt. Defaults to 8000.
        /// </summary>
        /// <returns></returns>
        int GetRetrySleepDuration();

        /// <summary>
        /// Get the time, in milliseconds, to wait before
        /// assuming a request isn't going to respond.
        /// Defaults to 3000.
        /// </summary>
        /// <returns></returns>
        int GetRequestTimeout();

        /// <summary>
        /// Get the number of exceptions that should occur
        /// before breaking the circuit. Defaults to 10.
        /// </summary>
        /// <returns></returns>
        int GetExceptionsBeforeBreakingCircuit();

        /// <summary>
        /// Get the duration, in milliseconds, to keep the
        /// circuit broken. Defaults to 8000.
        /// </summary>
        /// <returns></returns>
        int GetCircuitBreakerDuration();

        /// <summary>
        /// Get the time, in milliseconds, to wait for the 
        /// Kakfa producer to stream a message. Defaults to 100
        /// milliseconds.
        /// </summary>
        /// <returns></returns>
        int GetKafkaProducerTimeout();
    }
}
