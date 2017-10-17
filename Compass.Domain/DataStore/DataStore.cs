using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compass.Domain.DataStore.Couchbase;
using Compass.Domain.Models;
using Compass.Shared;
using Couchbase;

namespace Compass.Domain.DataStore
{
    public class DataStore : IDataStore
    {
        private readonly ICouchbaseFactory _couchbaseFactory;
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly ICouchbaseClient _couchbaseClient;

        public DataStore(
            ICouchbaseFactory couchbaseFactory,
            ICompassEnvironment compassEnvironment,
            ICouchbaseClient couchbaseClient
            )
        {
            _couchbaseFactory = couchbaseFactory;
            _compassEnvironment = compassEnvironment;
            _couchbaseClient = couchbaseClient;
        }

        public async Task<T> GetByDocumentIdAsync<T>(string documentId)
            where T : Entity
        {
            var bucket = _couchbaseFactory.GetBucket();
            var documentResult = await bucket.GetDocumentAsync<T>(documentId);
            return documentResult.Content;
        }

        public async Task<T> GetByIdentifierAsync<T>(string identifier) where T : Entity
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
                $@"SELECT {bucketName}.* FROM `{bucketName}`
                AND identifier = '{identifier}'
                AND docType = '{nameof(T)}'";

            var queryResult = await _couchbaseClient.QueryAsync<T>(query);
            return queryResult.SingleOrDefault();
        }

        public async Task<T> InsertAsync<T>(T entity)
            where T : Entity
        {
            ValidateIdentifier(entity);

            var document = new Document<T>()
            {
                Id = entity.Identifier.ToString(),
                Content = entity
            };

            var bucket = _couchbaseFactory.GetBucket();
            await bucket.InsertAsync(document);
            return entity;
        }

        public async Task<T> UpsertAsync<T>(T entity)
            where T : Entity
        {
            return await UpsertAsync(entity, null);
        }

        public async Task<T> UpsertAsync<T>(T entity, uint? ttl)
            where T : Entity
        {
            ValidateIdentifier(entity);

            var document = new Document<T>()
            {
                Id = entity.Identifier.ToString(),
                Content = entity
            };

            if (ttl.HasValue)
            {
                document.Expiry = ttl.Value;
            }

            var bucket = _couchbaseFactory.GetBucket();
            await bucket.UpsertAsync(document);
            return entity;
        }

        private static void ValidateIdentifier<T>(T entity) where T : Entity
        {
            if (entity.Identifier == Guid.Empty)
            {
                throw new Exception("No identifier has been set on the entity.");
            }
        }

        public async Task<ICollection<RegisteredApplication>> GetAllRegisteredApplicationsAsync()
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
                $@"SELECT {bucketName}.* FROM `{bucketName}`
                WHERE docType = '{nameof(RegisteredApplication)}'";

            var queryResult = await _couchbaseClient.QueryAsync<RegisteredApplication>(query);
            return queryResult.ToList();
        }

        public async Task<RegisteredApplication> GetRegisteredApplicationAsync(string applicationName)
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
              $@"SELECT {bucketName}.* FROM `{bucketName}`
                WHERE applicationName = '{applicationName}'
                AND docType = '{nameof(RegisteredApplication)}'";

            var queryResult = await _couchbaseClient.QueryAsync<RegisteredApplication>(query);
            return queryResult.SingleOrDefault();
        }

        public async Task<RegisteredApplication> GetRegisteredApplicationAsync(Guid applicationToken)
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
                $@"SELECT {bucketName}.* FROM `{bucketName}`
                WHERE applicationToken = '{applicationToken}'
                AND docType = '{nameof(RegisteredApplication)}'";

            var queryResult =  await _couchbaseClient.QueryAsync<RegisteredApplication>(query);
            return queryResult.SingleOrDefault();
        }

        public async Task<ICollection<ServiceSubscription>> GetAllServiceSubscriptionAsync()
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
                $@"SELECT {bucketName}.* FROM `{bucketName}`
                WHERE docType = '{nameof(ServiceSubscription)}'";

            var queryResult = await _couchbaseClient.QueryAsync<ServiceSubscription>(query);
            return queryResult.ToList();
        }

        public async Task<ICollection<ServiceSubscription>> GetServiceSubscribedToEventAsync(string eventName)
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
                $@"SELECT {bucketName}.* FROM `{bucketName}`
                WHERE '{eventName}' IN subscribedEvents
                AND docType = '{nameof(ServiceSubscription)}'";

            var queryResult = await _couchbaseClient.QueryAsync<ServiceSubscription>(query);
            return queryResult.ToList();
        }

        public async Task<ServiceSubscription> GetServiceSubscriptionAsync(Guid applicationToken)
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
                $@"SELECT {bucketName}.* FROM `{bucketName}`
                WHERE applicationToken = '{applicationToken}' 
                AND docType = '{nameof(ServiceSubscription)}'";

            var queryResult = await _couchbaseClient.QueryAsync<ServiceSubscription>(query);
            return queryResult.SingleOrDefault();
        }

        public async Task<QueuedEvents> GetQueuedEventsAsync()
        {
            var bucketName = _compassEnvironment.GetCouchbaseBucketName();
            var query =
                $@"SELECT {bucketName}.* FROM `{bucketName}`
                WHERE docType = '{nameof(QueuedEvents)}'";

            var queryResult = await _couchbaseClient.QueryAsync<QueuedEvents>(query);
            return queryResult.SingleOrDefault();
        }
    }
}