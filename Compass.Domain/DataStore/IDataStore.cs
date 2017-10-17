using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.DataStore
{
    public interface IDataStore
    {
        Task<T> GetByDocumentIdAsync<T>(string documentId) where T : Entity;
        Task<T> GetByIdentifierAsync<T>(string identifier) where T : Entity;
        Task<T> InsertAsync<T>(T entity) where T : Entity;
        Task<T> UpsertAsync<T>(T entity) where T : Entity;
        Task<T> UpsertAsync<T>(T entity, uint? ttl) where T : Entity;

        Task<ICollection<RegisteredApplication>> GetAllRegisteredApplicationsAsync();
        Task<RegisteredApplication> GetRegisteredApplicationAsync(string applicationName);
        Task<RegisteredApplication> GetRegisteredApplicationAsync(Guid applicationToken);
        Task<ICollection<ServiceSubscription>> GetAllServiceSubscriptionAsync();
        Task<ICollection<ServiceSubscription>> GetServiceSubscribedToEventAsync(string eventName);
        Task<ServiceSubscription> GetServiceSubscriptionAsync(Guid applicationToken);
        Task<QueuedEvents> GetQueuedEventsAsync();
    }
}
